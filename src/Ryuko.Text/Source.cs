
namespace Ryuko.Text
{
    using System;
    using System.Collections.Generic;
    using System.Resources;
    using System.Resources.Tools;

    public class Source
    {
        internal string _raw;


        public Source(string raw)
        {
            this._raw = raw; 
        }

        internal void Broadcast(object sender, int offset)
        {
            this.OnPushbacks?.Invoke(sender, offset);
        }

        internal event EventHandler<int> OnPushbacks;

        public Segment this[int index, int length]
        {
            get
            {
                return new Segment(index, this._raw.Substring(index, length), this);
            }
        }

        public Segment this[int index]
        {
            get
            {
                return new Segment(index, this._raw.Substring(index), this);
            }
        }


        public override string ToString() => this._raw.ToString();
    }

}


