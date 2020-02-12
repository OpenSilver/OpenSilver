
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



#if WCF_STACK

#if UNIMPLEMENTED_MEMBERS
namespace System.ServiceModel
{
    public sealed partial class OperationContextScope : IDisposable
    {
        private readonly OperationContext _originalContext = OperationContext.Current;
        private readonly OperationContextScope _originalScope = _currentScope;

        [ThreadStatic]
        private static OperationContextScope _currentScope;

        private OperationContext _currentContext;
        private bool _disposed;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="T:System.ServiceModel.OperationContextScope"/> qui utilise le <see cref="T:System.ServiceModel.IContextChannel"/> spécifié pour créer un <see cref="T:System.ServiceModel.OperationContext"/> pour la portée.
        /// </summary>
        /// <param name="channel">Le canal à utiliser lors de la création de la portée pour un nouveau <see cref="T:System.ServiceModel.OperationContext"/>.</param>
        public OperationContextScope(IContextChannel channel)
        {
            PushContext(new OperationContext(channel));
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="T:System.ServiceModel.OperationContextScope"/> pour créer une portée pour l'objet <see cref="T:System.ServiceModel.OperationContext"/> spécifié.
        /// </summary>
        /// <param name="context">Le <see cref="T:System.ServiceModel.OperationContext"/> actif dans la portée créée.</param>
        public OperationContextScope(OperationContext context)
        {
            PushContext(context);
        }

        /// <summary>
        /// Rétablit le <see cref="T:System.ServiceModel.OperationContext"/> d'origine comme contexte actif et recycle l'objet <see cref="T:System.ServiceModel.OperationContextScope"/>.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            PopContext();
        }

        private void PushContext(OperationContext context)
        {
            _currentContext = context;
            _currentScope = this;
            OperationContext.Current = _currentContext;
        }

        private void PopContext()
        {
            _currentScope = _originalScope;
            OperationContext.Current = _originalContext;
        }
    }
}
#endif

#endif