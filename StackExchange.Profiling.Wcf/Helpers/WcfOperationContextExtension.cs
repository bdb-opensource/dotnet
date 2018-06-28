namespace StackExchange.Profiling.Wcf.Helpers
{
    using System;
    using System.Collections;
    using System.Diagnostics.Contracts;
    using System.ServiceModel;

    /// <summary>
    /// <c>Taken from http://blog.caraulean.com/2008/02/13/httpcontext-idiom-for-windows-communication-foundation/</c>
    /// </summary>
    internal class WcfOperationContextExtension : IExtension<OperationContext>
    {
        private readonly Hashtable _items = new Hashtable();

        public static WcfOperationContextExtension Current
        {
            get
            {
                var context = OperationContext.Current;
                if (null == context)
                {
                    return null;
                }

                var extension = context.Extensions.Find<WcfOperationContextExtension>();
                if (null == extension)
                {
                    extension = new WcfOperationContextExtension();
                    context.Extensions.Add(extension);
                }

                return extension;
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        public IDictionary Items
        {
            get
            {
                Contract.Ensures(Contract.Result<IDictionary>() != null);

                return this._items;
            }
        }

        /// <summary>
        /// attach an instance.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Attach(OperationContext owner)
        {
        }

        /// <summary>
        /// detach an instance.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Detach(OperationContext owner)
        {
        }
    }
}