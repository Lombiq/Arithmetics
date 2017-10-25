using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lombiq.Arithmetics.Posit
{
    struct Posit
    {
        private readonly PositEnvironment _environment;
        public BitMask PositBits { get; }

        #region Posit structure

        public byte MaximumExponentSize => _environment.MaximumExponentSize;

        public ushort Size => _environment.Size;

        #endregion

        #region Posit constructors

        public Posit(PositEnvironment environment)
        {
            _environment = environment;

            PositBits = new BitMask(_environment.Size);
        }

        public Posit(PositEnvironment environment, BitMask bits)
        {
            _environment = environment;

            PositBits = BitMask.FromImmutableArray(bits.Segments, _environment.Size);
        }

        //public Posit(PositEnvironment environment, int value)
        //{
        //    _environment = environment;


        //}

        //public Posit(PositEnvironment environment, uint value)
        //{
        //    _environment = environment;


        //}

        #endregion

        #region Methods to handle parts of the Posit 

        

        #endregion
    }
}
