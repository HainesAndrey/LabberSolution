using MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;

namespace LabberClient.VMStuff
{
    public class Node : MvxViewModel
    {
        private string title;
        private uint idJournal;
        private bool isExpanded;

        public string Title { get => title; set { title = value; RaisePropertyChanged("Title"); } }
        public uint IdJournal { get => idJournal; set { idJournal = value; RaisePropertyChanged("IdJournal"); } }
        public bool IsExpanded { get => isExpanded;
            set
            {
                isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }
        public bool AreNodesAxpanded { get => Nodes.All(x => x.IsExpanded);
            set
            {
                ChangeNodesExpandedState(this, value);
                RaisePropertyChanged("AreNodesAxpanded");
            }
        }

        public ObservableCollection<Node> Nodes { get; set; }

        private void ChangeNodesExpandedState(Node node, bool state)
        {
            node.IsExpanded = state;
            if (node.Nodes != null)
                foreach (var child in node.Nodes)
                    ChangeNodesExpandedState(child, state);
        }
    }
}