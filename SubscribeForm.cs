using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace web2book
{
    public partial class SubscribeForm : Form
    {
        class PublishedSourceBindingClass : BindingList<PublishedSource>, ITypedList
        {
            private PropertyDescriptorCollection description = null;

            #region ITypedList Members

            public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
            {
                if (description == null)
                {
                    string[] fields = new string[] { "Enabled", "Name", "Comment", "Added", "Contributor" };
                    PropertyDescriptorCollection tmp = TypeDescriptor.GetProperties(typeof(PublishedSource)).Sort(fields);
                    description = new PropertyDescriptorCollection(null);
                    foreach (PropertyDescriptor pd in tmp)
                    {
                        foreach (string s in fields)
                        {
                            if (s == pd.Name)
                            {
                                description.Add(pd);
                            }
                        }
                    }
                }
                return description;
            }

            public string GetListName(PropertyDescriptor[] listAccessors)
            {
                return typeof(PublishedSource).Name;
            }

            #endregion

        }

        PublishedSourceBindingClass sources = new PublishedSourceBindingClass();
        ArrayList sourcesToAdd = new ArrayList();

        public ArrayList SourcesToAdd
        {
            get { return sourcesToAdd; }
        }

        public SubscribeForm(ContentSourceList currentSources)
        {
            InitializeComponent();
            PopulateSources(currentSources);
            sourcesDataGridView.DataSource = sources;
            this.Text = "Subscribe to " + currentSources.Source.SourceName+"s";
        }

        void PopulateSources(ContentSourceList currentSources)
        {
            ArrayList f = Utils.GetPublishedSources(Settings.Default.Username, Settings.Default.Password);
            foreach (PublishedSource s in f)
            {
                if (s.Source.ElementName != currentSources.Source.ElementName || currentSources.HasSource(s.Source))
                    continue;
                sources.Add(s);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void subscribeButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < sources.Count; i++)
            {
                if (sources[i].Enabled)
                {
                    sourcesToAdd.Add(sources[i].Source);
                }
            }
            Close();
        }
    }
}