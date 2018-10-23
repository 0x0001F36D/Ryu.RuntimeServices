// Author: Viyrex(aka Yuyu)
// Contact: mailto:viyrex.aka.yuyu@gmail.com
// Github: https://github.com/0x0001F36D
#define DLR
namespace Ryuko.RuntimeServices.DLR
{
    using CuttingEdge.Conditions;

    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

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
}