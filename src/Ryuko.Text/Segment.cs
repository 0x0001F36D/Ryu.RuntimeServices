
namespace Ryuko.Text
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Text.RegularExpressions;

    public class Segment
    {
        private volatile int _start;
        private readonly int _length;
        private Source _src;

        private string _seg;
        private readonly Lazy<StringBuilder> _builder;

        internal Segment(int index, string seg, Source src)
        {
            this._start = index;
            this._length = seg.Length;
            this._src = src;
            this._seg = seg;
            this._src.OnPushbacks += this.OnPushBack;
            this._builder = new Lazy<StringBuilder>(() => new StringBuilder());
        }

        private void OnPushBack(object sender, int offset)
        {
            this._start += offset;
        }

        public Segment Insert(int index, string value)
        {
            this._seg = this._seg.Insert(index, value);
            return this;
        }

        public Segment Delete(int index, int length)
        {
            this._seg = this._seg.Remove(index, length);
            return this;
        }
        public Segment Delete(int index)
        {
            this._seg = this._seg.Remove(index);
            return this;
        }

        public Segment Replace(string old, string @new, params (string old, string @new)[] list)
        {
            if (string.IsNullOrEmpty(old))
                return this;
            else
            {
                this._builder.Value.Append(this._seg);
                this._builder.Value.Replace(old, @new);

                foreach (var rule in list)
                {
                    if (string.IsNullOrEmpty(rule.old))
                        continue;
                    this._builder.Value.Replace(rule.old, rule.@new);
                }
                this._seg = this._builder.Value.ToString();
                this._builder.Value.Clear();
            }

            return this;
        }

        public Segment Replace(string regex, MatchEvaluator evaluator, RegexOptions? regexOptions = null, TimeSpan? matchTimeout = null)
        {
            this._seg = Regex.Replace(this._seg, regex, evaluator, regexOptions ?? RegexOptions.None, matchTimeout ?? Regex.InfiniteMatchTimeout);
            return this;
        }

        public async Task<Segment> ReplaceChain(params (string regex, MatchEvaluator evaluator, RegexOptions? regexOptions, TimeSpan? matchTimeout)[] set)
        {
            await Task.Run(() =>
            {
                foreach (var s in set)
                {
                    this._seg = Regex.Replace(this._seg,
                        s.regex,
                        s.evaluator,
                        s.regexOptions ?? RegexOptions.None,
                        s.matchTimeout ?? Regex.InfiniteMatchTimeout);
                }
            });

            return this;
        }


        public Source Rebase()
        {
            return new Source(this._seg);
        }


        public Source PushBack()
        {
            this._src._raw = this._src._raw
                .Remove(this._start, this._length)
                .Insert(this._start, this._seg);

            var offset = this._length - this._seg.Length;

            this._src.Broadcast(this, offset);
            this._src.OnPushbacks -= this.OnPushBack;
            return this._src;
        }


        public static Segment operator +(Segment segment, string other)
        {
            segment._seg = string.Concat(segment._seg, other);
            return segment;
        }
        public static Segment operator +(string other, Segment segment)
        {
            segment._seg = string.Concat(other, segment._seg);
            return segment;
        }

        public static Segment operator *(Segment segment, uint mul)
        {
            switch (mul)
            {
                case 0:
                    return segment.Delete(0);
                case 1:
                    return segment;

                default:
                    while (mul-- == 0)
                    {
                        segment._builder.Value.Append(segment._seg);
                    }
                    segment._seg = segment._builder.Value.ToString();
                    segment._builder.Value.Clear();
                    return segment;
            }
        }

    }

}


