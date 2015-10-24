﻿using LaGeBiaoQing.Model;
using LaGeBiaoQing.Service;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace LaGeBiaoQing.View.ComboBoxes
{
    enum TagContentComboBoxType { InCollectionTabPage, InDiscoverTabPage }

    public delegate void SelectTagContentEventHandler(object sender, TagContent selectTagContent);

    class TagContentComboBox : ComboBox
    {
        private TagContentComboBoxType type;
        private BackgroundWorker worker;
        private List<TagContent> tagContents;

        public event SelectTagContentEventHandler SelectTagContent;

        public TagContentComboBox(TagContentComboBoxType type)
        {
            this.type = type;

            SelectedIndexChanged += TagContentComboBox_SelectedIndexChanged;

            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void TagContentComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (type)
            {
                case TagContentComboBoxType.InDiscoverTabPage:
                    if (SelectedIndex > 0)
                    {
                        SelectTagContent(this, tagContents[SelectedIndex]);
                    }
                    break;
            }
        }

        public void loadTagContents()
        {
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            tagContents = TagService.GetAllTagContents();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // calculate data
            List<string> list = new List<string>();
            switch (type)
            {
                case TagContentComboBoxType.InCollectionTabPage:
                    list.Add("最近");
                    foreach (TagContent tagContent in tagContents)
                    {
                        list.Add((tagContent.content.Length > 0 ? tagContent.content : "默认") + "(" + tagContent.useAmount + ")");
                    }
                    break;
                case TagContentComboBoxType.InDiscoverTabPage:
                    list.Add("最新");
                    foreach (TagContent tagContent in tagContents)
                    {
                        if (tagContent.content.Length > 0)
                        {
                            list.Add(tagContent.content + "(" + tagContent.useAmount + ")");
                        }
                    }
                    break;
            }

            // modify UI
            Items.Clear();
            DataSource = list;
            DisplayMember = list[0];
        }

    }
}
