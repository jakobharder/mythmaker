using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace MythMaker.Rendering
{
    public enum CardRenderType
    {
        FullFront,
        FullBack,
        CutFront,
        CutBack
    }

    public interface IRenderCard
    {
        void Render(CardRenderType renderType, bool invoke = true);
    }

    public class RenderWorker
    {
        private static readonly BackgroundWorker worker = new BackgroundWorker();
        private static List<Tuple<IRenderCard, CardRenderType>> renderables = new List<Tuple<IRenderCard, CardRenderType>>();

        public static RenderWorker Instance = new RenderWorker();

        public RenderWorker()
        {
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // could be done much nicer, but does it's job for now

            // endlessly render
            while (true)
            {
                lock (this)
                {
                    if (!paused)
                    {
                        Tuple<IRenderCard, CardRenderType> next = null;
                        lock (renderables)
                        {
                            if (renderables.Count > 0)
                            {
                                next = renderables.First();
                                renderables = renderables.Distinct().ToList();
                                renderables.Remove(next);
                            }
                        }

                        if (next != null)
                        {
                            next.Item1.Render(next.Item2);
                        }
                    }
                }

                // no need to max out
                Thread.Sleep(paused ? 300 : 100);
            };
        }

        public void EnqueueCardUpdate(IRenderCard card, CardRenderType renderType)
        {
            lock (renderables)
                renderables.Add(new Tuple<IRenderCard, CardRenderType>(card, renderType));
        }

        public void Clear()
        {
            lock (this)
            {
                lock (renderables)
                    renderables.Clear();
            }
        }

        bool paused = false;
        public void Pause()
        {
            lock (this)
            {
                paused = true;
            }
        }

        public void Continue()
        {
            lock (this)
            {
                paused = false;
            }
        }
    }
}
