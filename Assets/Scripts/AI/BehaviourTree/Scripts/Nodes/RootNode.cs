using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class RootNode : Node {
        public Node child;

        protected override State OnUpdate() {
            return child.Update();
        }

        public override Node Clone() {
            RootNode node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }