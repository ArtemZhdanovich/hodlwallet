﻿using System;

using SkiaSharp;
using QRCoder;

using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace HodlWallet.Core.Utils
{
    public class QRCode : SKCanvasView
    {
        #region Fields

        private QRCodeData data;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SkiaSharp.QRCode"/> class.
        /// </summary>
        public QRCode()
        {
            this.PaintSurface += OnPaintSurface;
        }

        #endregion

        #region Dependency properties

        public static readonly BindableProperty LevelProperty = BindableProperty.Create(
            propertyName: nameof(Level),
            returnType: typeof(QRCodeGenerator.ECCLevel),
            declaringType: typeof(QRCode),
            defaultValue: QRCodeGenerator.ECCLevel.H,
            propertyChanged: OnInvalidationChanged);

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(
            propertyName: nameof(Color),
            returnType: typeof(Color),
            declaringType: typeof(QRCode),
            defaultValue: Color.Black,
            propertyChanged: OnInvalidationChanged);

        public static readonly BindableProperty ContentProperty = BindableProperty.Create(
           propertyName: nameof(Content),
           returnType: typeof(string),
           declaringType: typeof(QRCode),
           defaultValue: null,
           propertyChanged: OnContentChanged);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the content of the QRCode.
        /// </summary>
        /// <value>The content.</value>
        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color used to paint the QRCode modules.
        /// </summary>
        /// <value>The color.</value>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ECC level.
        /// </summary>
        /// <value>The level.</value>
        public QRCodeGenerator.ECCLevel Level
        {
            get { return (QRCodeGenerator.ECCLevel)GetValue(LevelProperty); }
            set { this.SetValue(LevelProperty, value); }
        }

        #endregion

        #region Methods

        private static void OnInvalidationChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is QRCode qrcode)
            {
                qrcode.InvalidateSurface();
            }
        }

        private static void OnContentChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is QRCode qrcode)
            {
                if (newValue is string content)
                {
                    using var generator = new QRCodeGenerator();
                    qrcode.data = generator.CreateQrCode(content, qrcode.Level);
                }
                else
                {
                    qrcode.data = null;
                }

                qrcode.InvalidateSurface();
            }
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.Transparent);

            if (this.data != null)
            {
                using var renderer = new QRCodeRenderer();
                renderer.Paint.Color = this.Color.ToSKColor();
                var area = SKRect.Create(0, 0, e.Info.Width, e.Info.Height);
                renderer.Render(e.Surface.Canvas, area, this.data);
            }
        }

        #endregion
    }
}