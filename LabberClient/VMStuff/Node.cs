using System.Collections.ObjectModel;

namespace LabberClient.VMStuff
{
    public class Node
    {
        public string Title { get; set; }
        public uint IdJournal { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }

        public bool IsExpanded { get; set; }

        private void ChangeNodesExpandedState(Node node, bool state)
        {
            IsExpanded = state;
            foreach (var child in Nodes)
                child.ChangeNodesExpandedState(node, state);
        }
    }
}