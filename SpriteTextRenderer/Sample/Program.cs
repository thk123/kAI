using System;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.D3DCompiler;
using Device = SlimDX.Direct3D11.Device;
using SpriteTextRenderer;
using SlimDX.DirectWrite;


namespace Sample
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var form = new RenderForm("SlimDX - MiniTri Direct3D 11 Sample");
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device device;
            SwapChain swapChain;
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out device, out swapChain);

            device.Factory.SetWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            Texture2D backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            var renderView = new RenderTargetView(device, backBuffer);            

            device.ImmediateContext.OutputMerger.SetTargets(renderView);
            device.ImmediateContext.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));            

            var sprite = new SpriteRenderer(device);
            var textBlock = new TextBlockRenderer(sprite, "Arial", FontWeight.Bold, SlimDX.DirectWrite.FontStyle.Normal, FontStretch.Normal, 16);

            MessagePump.Run(form, () =>
            {
                device.ImmediateContext.ClearRenderTargetView(renderView, Color.DarkBlue);

                textBlock.DrawString("ABCDEFGHIJKLMNOPQRSTUVWXYZ" + Environment.NewLine + "abcdefghijklmnopqrstuvwxyz", Vector2.Zero, new Color4(1.0f, 1.0f, 0.0f));

                sprite.Flush();

                swapChain.Present(0, PresentFlags.None);
            });
            textBlock.Dispose();
            sprite.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            device.Dispose();
            swapChain.Dispose();
        }

    }
}
