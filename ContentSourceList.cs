using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace web2book
{
    public class ContentSourceList
    {
        IContentSourceList dataSource;

        ISource source;

        public ISource Source
        {
            get { return source; }
        }

        TabPage tabPage;

        public TabPage TabPage
        {
            get { return tabPage; }
        }

        DataGridView dataGridView;

        public DataGridView DataGrid
        {
            get { return dataGridView; }
        }

        public int Count
        {
            get { return dataSource.Count; }
        }

        public ContentSource this[int index]
        {
            get { return dataSource[index]; }
        }

        public void Add(ContentSource cs)
        {
            dataSource.Add(cs);
        }

        public void ResetBindings()
        {
            dataSource.ResetBindings();
        }

        public ContentSourceList(ISource source)
        {
            this.source = source;
            tabPage = new TabPage();
            tabPage.Text = Source.SourceName;
            dataGridView = new DataGridView();
            dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;//.DisplayedCells;
            dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridView.Margin = new System.Windows.Forms.Padding(10);
            dataGridView.RowHeadersVisible = false;
            dataGridView.DataSource = dataSource = source.DataSource;
            tabPage.Controls.Add(this.dataGridView);
            tabPage.ResumeLayout(false);
        }

        public bool HasSource(ContentSource s)
        {
            return dataSource.HasSource(s);
        }

        public void Write(XmlTextWriter xw)
        {
            xw.WriteStartElement(Source.ElementName + "s");
            source.Write(xw);
            for (int i = 0; i < Count; i++)
            {
                if (!this[i].IsEmpty)
                {
                    this[i].Write(xw);
                }
            }
            xw.WriteEndElement();
        }

        public ContentSource CurrentItem()
        {
            for (int i = 0; i < dataGridView.ColumnCount; i++)
                dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;

            // Get the currently selected item
            int r = dataGridView.CurrentCellAddress.Y;
            if (r >= 0 && r < Count)
            {
                return this[r];
            }
            return null;
        }

        #region Source deletion

        public void DeleteCurrent()
        {
            int r = DataGrid.CurrentCellAddress.Y;
            if (r >= 0 && r < Count)
            {
                dataSource.RemoveAt(r);
            }
        }
        #endregion
    }
}
