using Knapsack.Helpers;
using Knapsack.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Knapsack.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Instanciates an object of the class MainWindowViewModel
        /// </summary>
        private Problem problem;

        public MainWindowViewModel()
        {
            ItemConfig.Characteristics = 2;
            ItemConfig.Profits = 2;
            ItemConfig.Items = 20;
        }

        private ItemConfig itemConfig = new ItemConfig();
        /// <summary>
        /// Gets or sets the item configuration
        /// </summary>
        public ItemConfig ItemConfig
        {
            get { return itemConfig; }
            set { itemConfig = value; }
        }

        public int ItemCount
        {
            get { return ItemConfig.Items; }
            set
            {
                if (ItemConfig.Items != value)
                {
                    ItemConfig.Items = value;
                    OnPropertyChanged("ItemCount");
                }
            }
        }

        public int Characteristics
        {
            get { return ItemConfig.Characteristics; }
            set
            {
                if (ItemConfig.Characteristics != value)
                {
                    ItemConfig.Characteristics = value;
                    OnPropertyChanged("Characteristics");
                }
            }
        }

        public int Profits
        {
            get { return ItemConfig.Profits; }
            set
            {
                if (ItemConfig.Profits != value)
                {
                    ItemConfig.Profits = value;
                    OnPropertyChanged("Profits");
                }
            }
        }

        private Problem Problem
        {
            get { return problem;}
            set
            {
                problem = value;
                DataTable items = new DataTable();
                for (int i = 0; i < ItemConfig.Characteristics; i++)
                {
                    items.Columns.Add(string.Format("c{0}", i + 1), typeof(double));
                }
                for (int i = 0; i < ItemConfig.Profits; i++)
                {
                    items.Columns.Add(string.Format("p{0}", i + 1), typeof(double));
                }
                foreach (Item item in problem.Items)
                {
                    var row = item.Characteristics.Concat(item.Profits).ToArray().Cast<object>().ToArray();
                    items.Rows.Add(row);
                }

                Items = items;
            }
        }


        private DataTable items = new DataTable();

        public DataTable Items
        {
            get { return items; }
            set
            {
                if (items != value)
                {
                    items = value;
                    OnPropertyChanged("Items");
                }
            }
        }

        private void Generate()
        {
            Problem = new Problem(ItemConfig);
        }

        private RelayCommand generateCmd;

        public RelayCommand GenerateCmd
        {
            get {
                if(generateCmd == null)
                {
                    generateCmd = new RelayCommand(p => Generate());
                }
                return generateCmd;
            }
            set { generateCmd = value; }
        }

        private void Save()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, problem);
                stream.Close();
            }
                
        }

        private RelayCommand saveCmd;

        public RelayCommand SaveCmd
        {
            get
            {
                if (saveCmd == null)
                {
                    saveCmd = new RelayCommand(p => Save());
                }
                return saveCmd;
            }
            set { saveCmd = value; }
        }

        private void Load()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                var loadedModel =formatter.Deserialize(stream);
                stream.Close();
                Problem = (Problem)loadedModel;
            }
        }

        private RelayCommand loadCmd;

        public RelayCommand LoadCmd
        {
            get
            {
                if (loadCmd == null)
                {
                    loadCmd = new RelayCommand(p => Load());
                }
                return loadCmd;
            }
            set { loadCmd = value; }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
