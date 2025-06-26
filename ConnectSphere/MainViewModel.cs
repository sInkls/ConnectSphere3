using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace SocialAppViewer
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ITableDataFactory _factory;
        private readonly Dictionary<string, DataTable> _loadedTables = new();
        private KeyValuePair<string, DataTable>? _selectedTable;
        private DataGrid _currentGrid;

        public MainViewModel()
        {
            _factory = new PostgresTableDataFactory("Host=localhost;Port=5432;Username=postgres;Password=12345;Database=ConnectSphere2");
            LoadTables();
        }

        public List<KeyValuePair<string, DataTable>> TableNames => new List<KeyValuePair<string, DataTable>>(_loadedTables);

        public KeyValuePair<string, DataTable>? SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (!_selectedTable.Equals(value))
                {
                    _selectedTable = value;
                    OnPropertyChanged();
                    UpdateCurrentView();
                }
            }
        }

        public object CurrentView
        {
            get => _currentGrid;
            set
            {
                if (_currentGrid != value)
                {
                    _currentGrid = (DataGrid)value;
                    OnPropertyChanged();
                }
            }
        }

        private void LoadTables()
        {
            var tableNames = new List<string>
            {
                "users", "profiles_users", "privacy_settings", "groups",
                "publications", "media_publications", "likes", "comments",
                "reposts", "friendships", "friend_requests", "subscriptions",
                "group_members", "requests_to_join_groups", "chats", "chats_members",
                "messages", "media_messages", "notifications", "notification_settings",
                "complaints", "blocking", "tags", "publications_tags", "group_tags"
            };

            foreach (var tableName in tableNames)
            {
                try
                {
                    var dataTable = _factory.LoadTable(tableName);
                    PrepareDataTable(dataTable, tableName);
                    _loadedTables[tableName] = dataTable;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка загрузки таблицы {tableName}: {ex.Message}");
                }
            }
        }

        private void PrepareDataTable(DataTable dataTable, string name)
        {
            dataTable.TableName = name;
        }

        private void UpdateCurrentView()
        {
            if (_selectedTable.HasValue)
            {
                var dataTable = _selectedTable.Value.Value;
                _currentGrid = new DataGrid
                {
                    ItemsSource = dataTable.DefaultView,
                    AutoGenerateColumns = true,
                    IsReadOnly = false
                };
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public void AddRow()
        {
            if (_selectedTable == null || !_selectedTable.HasValue)
            {
                return;
            }

            var dataTable = _selectedTable.Value.Value;

            if (dataTable.Columns.Count == 0)
            {
                return;
            }

            var pkColumn = GetPrimaryKeyColumn(_selectedTable.Value.Key);
            if (string.IsNullOrEmpty(pkColumn) || !dataTable.Columns.Contains(pkColumn))
            {
                Console.WriteLine($"Не удалось определить первичный ключ для таблицы {_selectedTable.Value.Key}.");
                return;
            }

            var columnType = dataTable.Columns[pkColumn].DataType;
            if (columnType != typeof(int) && columnType != typeof(long))
            {
                Console.WriteLine($"Столбец {pkColumn} таблицы {_selectedTable.Value.Key} не является целочисленным идентификатором.");
                return;
            }

            long maxId = 0;
            if (dataTable.Rows.Count > 0)
            {
                maxId = dataTable.AsEnumerable()
                    .Where(row => row.RowState != DataRowState.Deleted && row[pkColumn] != DBNull.Value)
                    .Max(row => Convert.ToInt64(row[pkColumn]));
            }

            var newRow = dataTable.NewRow();
            newRow[pkColumn] = maxId + 1;

            foreach (DataColumn column in dataTable.Columns)
            {
                if (!column.AllowDBNull && newRow[column] == DBNull.Value && column.ColumnName != pkColumn)
                {
                    newRow[column] = GetDefaultValue(column);
                }
            }

            dataTable.Rows.Add(newRow);
            OnPropertyChanged(nameof(CurrentView));
        }

        public void DeleteRow()
        {
            if (_currentGrid?.SelectedItem is DataRowView rowView)
            {
                rowView.Row.Delete();
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public void SaveChanges()
        {
            if (_selectedTable == null || !_selectedTable.HasValue)
            {
                return;
            }

            try
            {
                _factory.SaveTable(_selectedTable.Value.Key, _selectedTable.Value.Value);
                Console.WriteLine("Изменения сохранены в базе данных.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        public void ExportJson()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON|*.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(_loadedTables, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(saveFileDialog.FileName, json);
                Console.WriteLine("Экспорт завершен.");
            }
        }

        public void ImportJson()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON|*.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var json = System.IO.File.ReadAllText(openFileDialog.FileName);
                var tables = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DataTable>>(json);

                _loadedTables.Clear();
                LoadTables(); 

                foreach (var kvp in tables)
                {
                    PrepareDataTable(kvp.Value, kvp.Key);
                    _loadedTables[kvp.Key] = kvp.Value;
                }

                OnPropertyChanged(nameof(TableNames));
                SelectedTable = _loadedTables.FirstOrDefault().Key != null
                    ? new KeyValuePair<string, DataTable>(_loadedTables.FirstOrDefault().Key, _loadedTables.FirstOrDefault().Value)
                    : (KeyValuePair<string, DataTable>?)null;
            }
        }

        private string GetPrimaryKeyColumn(string tableName)
        {
            return tableName.ToLower() switch
            {
                "users" => "user_id",
                "profiles_users" => "profile_id",
                "privacy_settings" => "settings_id",
                "groups" => "group_id",
                "publications" => "post_id",
                "media_publications" => "media_id",
                "likes" => "like_id",
                "comments" => "comment_id",
                "reposts" => "repost_id",
                "friendships" => "friendship_id",
                "friend_requests" => "request_id",
                "subscriptions" => "subscription_id",
                "group_members" => "membership_id",
                "requests_to_join_groups" => "join_request_id",
                "chats" => "chat_id",
                "chats_members" => "chat_participant_id",
                "messages" => "message_id",
                "media_messages" => "media_message_id",
                "notifications" => "notification_id",
                "notification_settings" => "notification_settings_id",
                "complaints" => "complaint_id",
                "blocking" => "block_id",
                "tags" => "tag_id",
                "publications_tags" => "post_id",
                "group_tags" => "group_id",
                _ => null
            };
        }

        private object GetDefaultValue(DataColumn column)
        {
            if (column.DataType == typeof(int) || column.DataType == typeof(long))
                return 0;
            if (column.DataType == typeof(string))
                return string.Empty;
            if (column.DataType == typeof(DateTime))
                return DateTime.Now;
            return DBNull.Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}