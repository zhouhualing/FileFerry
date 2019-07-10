using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics;

namespace WD.Library.Reflection
{
	public static class EventInfoExtensions
	{
        
		public static Delegate CreateHandler(this EventInfo eventInfo, Action action)
		{
            Debug.Assert(eventInfo != null && action != null);

            Type handlerType = eventInfo.EventHandlerType;
			ParameterInfo[] eventParams = handlerType.GetMethod("Invoke").GetParameters();
			//lambda: (object x0, EventArgs x1) => d()
			IEnumerable<ParameterExpression> parameters
				= eventParams.Select(p => System.Linq.Expressions.Expression.Parameter(p.ParameterType, "x"));
			// - assumes void method with no arguments but can be
			//   changed to accomodate any supplied method
			MethodCallExpression body = System.Linq.Expressions.Expression.Call(
                System.Linq.Expressions.Expression.Constant(action), action.GetType().GetMethod("Invoke"));
			LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(body, parameters.ToArray());
			Delegate result = Delegate.CreateDelegate(handlerType, lambda.Compile(), "Invoke", false);
            
			return result;
		}
	}
}
