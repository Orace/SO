using System.Linq.Expressions;

namespace SO_72276645;

public static class ParameterReplacer
{
    // Produces an expression identical to 'expression'
    // except with 'source' parameter replaced with 'target' expression.     
    public static LambdaExpression Replace<T>(Expression<T> expression,
                                              ParameterExpression source,
                                              Expression target)
    {
        return new ParameterReplacerVisitor(source, target).VisitAndConvert(expression);
    }

    private class ParameterReplacerVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _source;
        private readonly Expression _target;

        public ParameterReplacerVisitor(ParameterExpression source, Expression target)
        {
            _source = source;
            _target = target;
        }

        internal LambdaExpression VisitAndConvert<T>(Expression<T> root)
        {
            return VisitLambda(root);
        }

        protected override LambdaExpression VisitLambda<T>(Expression<T> node)
        {
            // Leave all parameters alone except the one we want to replace.
            var parameters = node.Parameters.Where(p => p != _source);

            return Expression.Lambda(Visit(node.Body), parameters);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            // Replace the source with the target, visit other params as usual.
            return node == _source ? _target : base.VisitParameter(node);
        }
    }
}