using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMaNGOSNET.Common.Network
{
    public interface IDataHoldingUserToken
    {
        Guid DataHoldingUserTokenGUID
        {
            get;
        }

        int ReceiveOffset
        {
            get;
        }

        int SendOffset
        {
            get;
        }

        int ReceiveBufferSize
        {
            get;
        }

        int SendBufferSize
        {
            get;
        }

        int SendSize
        {
            get;
        }

        void ResetReceivedCount();

        void ResetSendSize();

        int ProcessedReceiveDataOffset
        {
            get;
            set;
        }

        int RemainingReceiveDataCount
        {
            get;
            set;
        }

        int RemainingReceiveDataBufferLength
        {
            get;
        }
    }
}
