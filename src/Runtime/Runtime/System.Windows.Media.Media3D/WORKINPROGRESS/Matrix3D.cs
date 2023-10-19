
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

namespace System.Windows.Media.Media3D
{
	[OpenSilver.NotImplemented]
    public struct Matrix3D : IFormattable
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
        
		[OpenSilver.NotImplemented]
        public double M11
        {
            get { return _m11; }
            set { _m11 = value; }
        }
		[OpenSilver.NotImplemented]
        public double M12
        {
            get { return _m12; }
            set { _m12 = value; }
        }
		[OpenSilver.NotImplemented]
        public double M13
        {
            get { return _m13; }
            set { _m13 = value; }
        }
		[OpenSilver.NotImplemented]
        public double M14
        {
            get { return _m14; }
            set { _m14 = value; }
        }
		[OpenSilver.NotImplemented]
        public double M21
        {
            get { return _m21; }
            set { _m21 = value; }
        }
		[OpenSilver.NotImplemented]
        public double M22
        {
            get { return _m22; }
            set { _m22 = value; }
        }
		[OpenSilver.NotImplemented]
        public double M23
        {
            get { return _m23; }
            set { _m23 = value; }
        }
		[OpenSilver.NotImplemented]
        public double M24
        {
            get { return _m24; }
            set { _m24 = value; }
        }
		[OpenSilver.NotImplemented]
        public double M31
        {
            get { return _m31; }
            set { _m31 = value; }
        }
		[OpenSilver.NotImplemented]
        public double M32
        {
            get { return _m32; }
            set { _m32 = value; }
        }
		[OpenSilver.NotImplemented]
        public double M33
        {
            get { return _m33; }
            set { _m33 = value; }
        }
		[OpenSilver.NotImplemented]
        public double OffsetX
        {
            get { return _offsetX; }
            set { _offsetX = value; }
        }
		[OpenSilver.NotImplemented]
        public double OffsetY
        {
            get { return _offsetY; }
            set { _offsetY = value; }
        }
		[OpenSilver.NotImplemented]
        public double OffsetZ
        {
            get { return _offsetZ; }
            set { _offsetZ = value; }
        }
		[OpenSilver.NotImplemented]
        public double M44
        {
            get { return _m44; }
            set { _m44 = value; }
        }
		[OpenSilver.NotImplemented]
        public static Matrix3D Identity
        {
            get { return _identity; }
        }
		[OpenSilver.NotImplemented]
        public bool HasInverse
        {
            get { return _hasInverse; }
        }
        

		[OpenSilver.NotImplemented]
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
		[OpenSilver.NotImplemented]
        public override string ToString()
        {
            return "";
        }
		[OpenSilver.NotImplemented]
        public string ToString(IFormatProvider @provider)
        {
            return "";
        }
        string IFormattable.ToString(string @format, IFormatProvider @provider)
        {
            return "";
        }
		[OpenSilver.NotImplemented]
        public static Matrix3D operator *(Matrix3D @matrix1, Matrix3D @matrix2)
        {
            return new Matrix3D();
        }
		[OpenSilver.NotImplemented]
        public void Invert()
        {
        }
        

    }
}
