using System;

namespace webapi
{
    public class SignatureVerifier
    {
        private IPublickeySource _pubkeySrc;

        public SignatureVerifier(IPublickeySource pubkeySrc) 
        {
            _pubkeySrc = pubkeySrc ?? throw new ArgumentNullException(nameof(pubkeySrc));
        }
        
        
    }
}