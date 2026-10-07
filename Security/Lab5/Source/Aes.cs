using Source.FileSystem;

namespace Source;

public class Aes
{
    private byte[] _sbox;

    public Aes()
    {
        _sbox = FileLoader.LoadSBox();
    }

   
    public byte[] Encrypt(byte[] text, byte[] key)
    {
        if (text.Length != 16) throw new ArgumentException("Text must be 16 bytes");
        if (key.Length != 16) throw new ArgumentException("Key must be 16 bytes");

        
        byte[,] state = PrepareState(text);

        
        byte[,] roundKey = GetRoundKey(key, roundIndex: 0);

       
        state = AddRoundKey(state, roundKey);
        state = SubBytes(state);
        state = ShiftRows(state);
        state = MixColumns(state);

        
        return StateToBytes(state);
    }

    
    private byte[,] PrepareState(byte[] text)
    {
        var state = new byte[4, 4];
        int rows = 4;
        int colms = 4;

        for (int i = 0; i < rows; i++)
        {
            for (int k = 0; k < colms; k++)
            {
                state[i, k] = text[k * rows + i];
            }
        }
        return state;
    }

    private byte[] StateToBytes(byte[,] state)
    {
        byte[] result = new byte[16];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                result[j * 4 + i] = state[i, j];
            }
        }
        return result;
    }

    
    private byte[,] AddRoundKey(byte[,] state, byte[,] roundKey)
    {
        var stateWithRoundKey = new byte[4, 4];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                stateWithRoundKey[i, j] = (byte)(state[i, j] ^ roundKey[i, j]);
            }
        }
        return stateWithRoundKey;
    }

    private byte[,] SubBytes(byte[,] state)
    {
        var stateWithSub = new byte[4, 4];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                stateWithSub[i, j] = (byte)_sbox[state[i, j]];
            }
        }
        return stateWithSub;
    }

   
    private byte[,] ShiftRows(byte[,] state)
    {
        var shifted = new byte[4, 4];

    
        shifted[0, 0] = state[0, 0];
        shifted[0, 1] = state[0, 1];
        shifted[0, 2] = state[0, 2];
        shifted[0, 3] = state[0, 3];

  
        shifted[1, 0] = state[1, 1];
        shifted[1, 1] = state[1, 2];
        shifted[1, 2] = state[1, 3];
        shifted[1, 3] = state[1, 0];

  
        shifted[2, 0] = state[2, 2];
        shifted[2, 1] = state[2, 3];
        shifted[2, 2] = state[2, 0];
        shifted[2, 3] = state[2, 1];

     
        shifted[3, 0] = state[3, 3];
        shifted[3, 1] = state[3, 0];
        shifted[3, 2] = state[3, 1];
        shifted[3, 3] = state[3, 2];

        return shifted;
    }

    private byte[,] MixColumns(byte[,] state)
    {
        var mixed = new byte[4, 4];

        for (int i = 0; i < 4; i++)
        {
            byte a0 = state[0, i];
            byte a1 = state[1, i];
            byte a2 = state[2, i];
            byte a3 = state[3, i];

            mixed[0, i] = (byte)(GaloisMultiply(a0, 2) ^ GaloisMultiply(a1, 3) ^ a2 ^ a3);
            mixed[1, i] = (byte)(a0 ^ GaloisMultiply(a1, 2) ^ GaloisMultiply(a2, 3) ^ a3);
            mixed[2, i] = (byte)(a0 ^ a1 ^ GaloisMultiply(a2, 2) ^ GaloisMultiply(a3, 3));
            mixed[3, i] = (byte)(GaloisMultiply(a0, 3) ^ a1 ^ a2 ^ GaloisMultiply(a3, 2));
        }

        return mixed;
    }

    
    private byte GaloisMultiply(byte a, byte b)
    {
        byte p = 0;
        byte counter;
        byte hi_bit_set;

        for (counter = 0; counter < 8; counter++)
        {
            if ((b & 1) == 1)
            {
                p ^= a;
            }

            hi_bit_set = (byte)(a & 0x80);
            a <<= 1;
            if (hi_bit_set == 0x80)
            {
                a ^= 0x1B; 
            }

            b >>= 1;
        }

        return p;
    }

    
    private byte[,] GetRoundKey(byte[] key, int roundIndex)
    {
       
        if (roundIndex == 0)
        {
            return PrepareState(key);
        }

        byte[][] w = new byte[4][];
        for (int i = 0; i < 4; i++)
        {
            w[i] = new byte[] { key[i * 4], key[i * 4 + 1], key[i * 4 + 2], key[i * 4 + 3] };
        }

        for (int i = 4; i < 8; i++)
        {
            byte[] temp = (byte[])w[i - 1].Clone();

            if (i % 4 == 0)
            {
               
                byte t = temp[0];
                temp[0] = temp[1];
                temp[1] = temp[2];
                temp[2] = temp[3];
                temp[3] = t;

                for (int j = 0; j < 4; j++)
                {
                    temp[j] = (byte)_sbox[temp[j]];
                }

             
                temp[0] ^= 0x01;
            }
         
            w[i % 4] = new byte[4];
            for (int j = 0; j < 4; j++)
            {
                w[i % 4][j] = (byte)(w[(i - 4) % 4][j] ^ temp[j]);
            }
        }
        
        byte[] roundKeyBytes = new byte[16];
        for (int i = 0; i < 4; i++)
        {
            Array.Copy(w[i], 0, roundKeyBytes, i * 4, 4);
        }

        return PrepareState(roundKeyBytes);
    }
}