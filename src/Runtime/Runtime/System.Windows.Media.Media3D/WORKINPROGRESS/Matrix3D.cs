
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Media.Media3D
#else
namespace Windows.UI.Xaml.Media.Media3D
#endif
{
    public partial struct Matrix3D : IFormattable
    {
        static Matrix3D()
        {
            _identity = new Matrix3D(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0);
        }

        private double _m11;
        private double _m12;
        private double _m13;
        private double _m14;
        private double _m21;
        private double _m22;
        private double _m23;
        private double _m24;
        private double _m31;
        private double _m32;
        private double _m33;
        private double _offsetX;
        private double _offsetY;
        private double _offsetZ;
        private double _m44;
        private static Matrix3D _identity;
        private bool _hasInverse;
        
        public double M11
        {
            get { return _m11; }
            set { _m11 = value; }
        }
        public double M12
        {
            get { return _m12; }
            set { _m12 = value; }
        }
        public double M13
        {
            get { return _m13; }
            set { _m13 = value; }
        }
        public double M14
        {
            get { return _m14; }
            set { _m14 = value; }
        }
        public double M21
        {
            get { return _m21; }
            set { _m21 = value; }
        }
        public double M22
        {
            get { return _m22; }
            set { _m22 = value; }
        }
        public double M23
        {
            get { return _m23; }
            set { _m23 = value; }
        }
        public double M24
        {
            get { return _m24; }
            set { _m24 = value; }
        }
        public double M31
        {
            get { return _m31; }
            set { _m31 = value; }
        }
        public double M32
        {
            get { return _m32; }
            set { _m32 = value; }
        }
        public double M33
        {
            get { return _m33; }
            set { _m33 = value; }
        }
        public double OffsetX
        {
            get { return _offsetX; }
            set { _offsetX = value; }
        }
        public double OffsetY
        {
            get { return _offsetY; }
            set { _offsetY = value; }
        }
        public double OffsetZ
        {
            get { return _offsetZ; }
            set { _offsetZ = value; }
        }
        public double M44
        {
            get { return _m44; }
            set { _m44 = value; }
        }
        public static Matrix3D Identity
        {
            get { return _identity; }
        }
        public bool HasInverse
        {
            get { return _hasInverse; }
        }
        

        public Matrix3D(double @m11, double @m12, double @m13, double @m14, double @m21, double @m22, double @m23, double @m24, double @m31, double @m32, double @m33, double @m34, double @offsetX, double @offsetY, double @offsetZ, double @m44)
        {
            _m11 = @m11;
            _m12 = @m12;
            _m13 = @m13;
            _m14 = @m14;
            _m21 = @m21;
            _m22 = @m22;
            _m23 = @m23;
            _m24 = @m24;
            _m31 = @m31;
            _m32 = @m32;
            _m33 = @m33;
            _offsetX = @offsetX;
            _offsetY = @offsetY;
            _offsetZ = @offsetZ;
            _m44 = @m44;
            _hasInverse = false;
        }
        public override string ToString()
        {
            return "";
        }
        public string ToString(IFormatProvider @provider)
        {
            return "";
        }
        string IFormattable.ToString(string @format, IFormatProvider @provider)
        {
            return "";
        }
        public static Matrix3D operator*(Matrix3D @matrix1, Matrix3D @matrix2)
        {
            return new Matrix3D();
        }
        public void Invert()
        {
        }
        

    }
}

#endif