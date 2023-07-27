using System.Linq.Expressions;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var foo = new Foo();
            //var paramter = Expression.Parameter(typeof(Foo), "f");
            //var bar = Expression.Property(paramter, "test");

            //object obj = null;
            //Enum.TryParse(typeof(TestEnum), "1", out obj);
            //var expr = Expression.Assign(bar, Expression.Constant(obj));
            //var e = Expression.Lambda<Action<Foo>>(expr, paramter).Compile();
            //e(foo);
            foo.DoSet(f => f.Bar, "bar");
            foo.DoSet(f => f.Bar2, "bar2");
            foo.DoSet(f => f.test, 1);
        }
    }
    public static class DoSomething
    {
        public static void DoSet<Foo, TProperty>(this Foo foo, Expression<Func<Foo, TProperty>> expression, dynamic val)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null && memberExpression.Member.MemberType == System.Reflection.MemberTypes.Property)
            {
                var objValue = GetValue(expression.Body.Type, val);
                SetValue(foo, memberExpression.Member.Name, objValue);
            }
        }

        private static void SetValue<Foo>(Foo? foo, string name, dynamic val)
        {
            var paramter = Expression.Parameter(typeof(Foo), "f");
            Expression.Assign(Expression.Property(paramter, name), Expression.Constant(val));
            var expr = Expression.Lambda<Action<Foo>>(Expression.Assign(Expression.Property(paramter, name), Expression.Constant(val)), paramter).Compile();
            expr(foo);
        }

        private static object GetValue(Type type, dynamic val)
        {
            if (type == typeof(string))
            {
                return val.ToString();
            }
            if (type == typeof(int))
            {
                var tryval = 0;
                if (int.TryParse(val, out tryval))
                {
                    return tryval;
                }
                return 0;
            }
            if (type.IsEnum)
            {
                object tryval;
                if (Enum.TryParse(type, val.ToString(), out tryval)) ;
                {
                    return tryval;
                }
            }
            return null;
        }
    }
    public class Foo
    {
        public string Bar
        {
            get; set;
        }
        public int Bar2
        {
            get; set;
        }
        public TestEnum test { get; set; }
    }
    public enum TestEnum
    {
        TT,
        LL
    }
}