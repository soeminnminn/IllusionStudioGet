using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Illusion.Card
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UnityColor
    {
        #region Members
        public float r;
        public float g;
        public float b;
        public float a;
        #endregion

        #region Constructor
        public UnityColor(float r, float g, float b)
            : this(r, g, b, 1f)
        { }

        public UnityColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        #endregion

        #region Properties
        public static UnityColor white
        {
            get => new UnityColor(1f, 1f, 1f, 1f);
        }

        public static UnityColor black
        {
            get => new UnityColor(0.0f, 0.0f, 0.0f, 1f);
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        #region Members
        public float x;
        public float y;
        public float z;
        #endregion

        #region Constructor
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0.0f;
        }
        #endregion

        #region Static
        public static Vector3 zero
        {
            get
            {
                return new Vector3(0.0f, 0.0f, 0.0f);
            }
        }

        public static Vector3 one
        {
            get
            {
                return new Vector3(1f, 1f, 1f);
            }
        }
        #endregion
    }
}
