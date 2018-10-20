// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D

namespace Viyrex.RuntimeServices.DLR
{
    using CuttingEdge.Conditions;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// 可對屬性做原子化操作
    /// </summary>
    public interface IAtomicity
    {
        bool Create<T>(string name, T value);

        bool Delete(string name);

        bool Exist(string name);

        bool Retrieve<T>(string name, out T value);

        bool Update<T>(string name, T value);
    }

    public static class Extension
    {
        public delegate object MemberSelector<T>(T target);

        public static dynamic ToSynthesis<T>(this T target, params Expression<MemberSelector<T>>[] selectors) where T : class
        {
            Condition.Requires(target)
                .IsNotNull();
            Condition.Ensures(selectors)
                .IsNotNull()
                .IsNotEmpty();

            var type = typeof(T);
            var items = new Dictionary<string, object>();
            foreach (var selector in selectors)
            {
                var body = selector.Body;
                // process default(struct)
                if (body is UnaryExpression ue)
                {
                    body = ue.Operand;
                }
                // end process

                if (body is MemberExpression me)
                {
                    var name = default(string);
                    var value = default(object);
                    switch (me.Member)
                    {
                        case PropertyInfo property:
                            {
                                name = property.Name;
                                value = property.GetValue(target);
                                break;
                            }
                        case FieldInfo field:
                            {
                                name = field.Name;
                                value = field.GetValue(target);
                                break;
                            }
                        default:
                            throw new NotSupportedException(selector.ToString());
                    }
                    items[name] = value;
                }
            }

            return new Synthesis(items);
        }
    }

    /// <summary>
    /// 提供一般物件至 DLR 物件的動態轉換
    /// </summary>
    [Serializable]
    public class Synthesis :
        IDynamicMetaObjectProvider,
        ISerializable,
        INotifyPropertyChanged,
        IEnumerable<KeyValuePair<string, object>>,
        IAtomicity
    {
        [Serializable]
        private sealed class DLRFields : DynamicObject, INotifyPropertyChanged, IDictionary<string, object>, ISerializable
        {
            private readonly Dictionary<string, object> _fields;

            public event PropertyChangedEventHandler PropertyChanged;

            public int Count => this._fields.Count;

            public bool IsReadOnly => ((IDictionary<string, object>)this._fields).IsReadOnly;

            public ICollection<string> Keys => this._fields.Keys;

            public ICollection<object> Values => this._fields.Values;

            public object this[string key] { get => this._fields[key]; set => this._fields[key] = value; }

            internal DLRFields(Dictionary<string, object> fields)
            {
                this._fields = fields;
            }

            public void Add(string key, object value) => this._fields.Add(key, value);

            public void Add(KeyValuePair<string, object> item) => ((IDictionary<string, object>)this._fields).Add(item);

            public void Clear() => this._fields.Clear();

            public bool Contains(KeyValuePair<string, object> item) => this._fields.Contains(item);

            public bool ContainsKey(string key) => this._fields.ContainsKey(key);

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => ((IDictionary<string, object>)this._fields).CopyTo(array, arrayIndex);

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this._fields.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this._fields.GetEnumerator();

            public void GetObjectData(SerializationInfo info, StreamingContext context) => this._fields.GetObjectData(info, context);

            public bool Remove(string key) => this._fields.Remove(key);

            public bool Remove(KeyValuePair<string, object> item) => ((IDictionary<string, object>)this._fields).Remove(item);

            public override bool TryDeleteMember(DeleteMemberBinder binder)
            {
                if (this._fields.ContainsKey(binder.Name))
                    this._fields.Remove(binder.Name);

                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                this._fields.TryGetValue(binder.Name, out result);
                return true;
            }

            public bool TryGetValue(string key, out object value) => this._fields.TryGetValue(key, out value);

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                this._fields[binder.Name] = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(binder.Name));

                return true;
            }
        }

        private sealed class DLRMetaObject : DynamicMetaObject
        {
            private readonly DynamicMetaObject _metaObject;

            internal DLRMetaObject(IDynamicMetaObjectProvider provider, Expression exp, BindingRestrictions restrictions, object value) : base(exp, restrictions, value) => this._metaObject = provider.GetMetaObject(Expression.Constant(provider));

            public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg) => this._metaObject.BindBinaryOperation(binder, arg);

            public override DynamicMetaObject BindConvert(ConvertBinder binder) => this._metaObject.BindConvert(binder);

            public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args) => this._metaObject.BindCreateInstance(binder, args);

            public override DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes) => this._metaObject.BindDeleteIndex(binder, indexes);

            public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder) => this._metaObject.BindDeleteMember(binder);

            public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes) => this._metaObject.BindGetIndex(binder, indexes);

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder) => this._metaObject.BindGetMember(binder);

            public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args) => this._metaObject.BindInvoke(binder, args);

            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args) => this._metaObject.BindInvokeMember(binder, args);

            public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value) => this._metaObject.BindSetIndex(binder, indexes, value);

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value) => this._metaObject.BindSetMember(binder, value);

            public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder) => this._metaObject.BindUnaryOperation(binder);

            public override IEnumerable<string> GetDynamicMemberNames() => this._metaObject.GetDynamicMemberNames();
        }

        private readonly IDynamicMetaObjectProvider _provider;

        /// <summary>
        /// 當物件成員的值變更時引發
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                ((INotifyPropertyChanged)this._provider).PropertyChanged += value;
            }
            remove
            {
                ((INotifyPropertyChanged)this._provider).PropertyChanged -= value;
            }
        }

        protected dynamic This => this;

        public Synthesis(out IAtomicity @this) : this()
        {
            @this = this;
        }

        /// <summary>
        /// 無建構參數的動態物件建構子
        /// </summary>
        public Synthesis() : this(null)
        {
        }

        /// <summary> 提供從 <see cref="IDictionary<string, object>"/> 轉換為動態物件的建構子 </summary> <param name="dictionary"></param>
        public Synthesis(IDictionary<string, object> dictionary)
        {
            var fields = default(Dictionary<string, object>);
            if (dictionary is null)
                fields = new Dictionary<string, object>();
            else if (dictionary is Dictionary<string, object> dict)
                fields = dict;
            else
                fields = new Dictionary<string, object>(dictionary);
            this._provider = new DLRFields(fields);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Add(string name, object value)
        {
            Condition.Requires(name, nameof(name))
                .IsNotNullOrWhiteSpace();

            this.Create(name, value);
        }

        /// <summary>
        /// 建立新動態物件成員
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">動態物件成員名稱</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Create<T>(string name, T value)
        {
            Condition.Requires(name, nameof(name))
                .IsNotNullOrWhiteSpace();

            if (this.Exist(name))
                return false;
            ((IDictionary<string, object>)this._provider).Add(name, value);
            return true;
        }

        /// <summary>
        /// 透過動態物件成員名稱來刪除成員
        /// </summary>
        /// <param name="name">動態物件成員名稱</param>
        /// <returns></returns>
        public bool Delete(string name)
        {
            Condition.Requires(name, nameof(name))
                .IsNotNullOrWhiteSpace();

            return ((IDictionary<string, object>)this._provider).Remove(name);
        }

        /// <summary>
        /// 取得動態物件成員名稱是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exist(string name)
        {
            Condition.Requires(name, nameof(name))
                .IsNotNullOrWhiteSpace();

            return ((IDictionary<string, object>)this._provider).ContainsKey(name);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => ((IDictionary<string, object>)this._provider).GetEnumerator();

        [EditorBrowsable(EditorBrowsableState.Never)]
        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<string, object>)this._provider).GetEnumerator();

        [EditorBrowsable(EditorBrowsableState.Never)]
        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) => new DLRMetaObject(this._provider, parameter, BindingRestrictions.GetTypeRestriction(parameter, this.GetType()), this);

        [EditorBrowsable(EditorBrowsableState.Never)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => ((ISerializable)this._provider).GetObjectData(info, context);

        /// <summary>
        /// 透過動態物件成員名稱，嘗試取回值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">動態物件成員名稱</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Retrieve<T>(string name, out T value)
        {
            Condition.Requires(name, nameof(name))
                .IsNotNullOrWhiteSpace();

            value = default(T);
            if (((IDictionary<string, object>)this._provider).TryGetValue(name, out var v) && v is T)
            {
                value = (T)v;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新動態物件成員的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">動態物件成員名稱</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Update<T>(string name, T value)
        {
            Condition.Requires(name, nameof(name))
                .IsNotNullOrWhiteSpace();

            if (this.Exist(name))
            {
                ((IDictionary<string, object>)this._provider)[name] = value;
                return true;
            }
            return false;
        }
    }
}