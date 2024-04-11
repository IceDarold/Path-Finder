using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generate.Bioms
{
    public abstract class BiomGenerator : Generator
    {
        public BiomGenerator(GenerateData generateData) : base(generateData) { }
    }
}
