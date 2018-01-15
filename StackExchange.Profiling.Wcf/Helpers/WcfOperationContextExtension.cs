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
        /// <summary>
        /// The items.
        /// </summary>
        private readonly IDictionary _items;

        /// <summary>
        /// Prevents a default instance of the <see cref="WcfInstanceContext"/> class from being created.
        /// </summary>
        private WcfOperationContextExtension()
        {
            _items = new Hashtable();
        }

        /// <summary>
        /// Gets the current <see cref="WcfOperationContextExtension"/>. Note that if
        /// this is only going to be used to read items, it is more <c>performant</c> to use
        /// <see cref="GetCurrentWithoutInstantiating"/> as this does not create and
        /// attach a new extension.
        /// </summary>
        public static WcfOperationContextExtension Current
        {
            get
            {
                var context = OperationContext.Current;
                if (null == context)
                {
                    throw new InvalidOperationException("An attempt was made to access a WCF operation context extension outside of WCF context.");
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

                return _items;
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