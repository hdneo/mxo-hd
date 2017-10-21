using System;
using System.Collections;

namespace hds
{
    public interface IWorldEncryption
    {
        byte[] encrypt(byte[] plainData, int length, UInt16 pss, UInt16 cseq, UInt16 sseq);
        ArrayList decrypt(byte[] encryptedData, int length);
    }
}