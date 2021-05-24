using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtopiaBot.Utopia
{
    class getFinanceSystemInformation
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Result
        {
            public bool PoS { get; set; }
            public int cardCreatePrice { get; set; }
            public int cardCreatePrice10 { get; set; }
            public int cardCreatePrice100 { get; set; }
            public int cardCreatePrice1000 { get; set; }
            public int cardCreatePrice10000 { get; set; }
            public bool cardsCreationEnabled { get; set; }
            public int cardsMaxActive { get; set; }
            public int cardsMaxPerDay { get; set; }
            public bool enableToUseMining { get; set; }
            public int invoicesDefaultTtl { get; set; }
            public bool invoicesEnabled { get; set; }
            public int invoicesMaxTotal { get; set; }
            public int invoicesMaxTotalFromMerchant { get; set; }
            public double invoicesMinAmount { get; set; }
            public int settingsVersion { get; set; }
            public double transferCardFee { get; set; }
            public bool transferCheckFee { get; set; }
            public double transferExternalFee { get; set; }
            public double transferInternalFee { get; set; }
            public bool transfersEnabled { get; set; }
            public int unsDefaultTtl { get; set; }
            public double unsDeleteNameFee { get; set; }
            public double unsModifyNameFee { get; set; }
            public int unsName1RegistrationFee { get; set; }
            public int unsName2RegistrationFee { get; set; }
            public int unsName3RegistrationFee { get; set; }
            public int unsName4RegistrationFee { get; set; }
            public bool unsProxyEnabled { get; set; }
            public double unsTransferFee { get; set; }
            public bool vouchersCreateEnabled { get; set; }
            public int vouchersMaxActive { get; set; }
            public int vouchersMaxPerBatch { get; set; }
            public double vouchersMinAmount { get; set; }
            public int vouchersMinPerBatch { get; set; }
            public bool vouchersUseEnabled { get; set; }
        }

        public class Root
        {
            public Result result { get; set; }
        }




    }
}
