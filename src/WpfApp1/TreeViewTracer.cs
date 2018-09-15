namespace WpfApp1
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using OpenTracing;
    using OpenTracing.Contrib.EventHookTracer;
    using OpenTracing.Mock;
    using OpenTracing.Util;

    internal static class TreeViewTracer
    {
        public static ITracer Create(TreeView tree)
        {
            var tracer = new EventHookTracer(new MockTracer());

            var stateHolder = new StatefulClass(tree);

            tracer.SpanActivated += stateHolder.TracerOnSpanActivated;
            tracer.SpanFinished += stateHolder.TracerOnSpanFinished;
            tracer.SpanLog += stateHolder.TracerOnSpanLog;
            tracer.SpanSetTag += stateHolder.TracerOnSpanSetTag;

            return tracer;
        }

        private sealed class StatefulClass
        {
            // TODO: If EventHookTracer exposed OnActivatING, since OT spec doesn't expose parent span, we could then observe them. Otherwise we have to track ourselves
            private readonly AsyncLocal<List<ISpan>> currentSpan = new AsyncLocal<List<ISpan>>();

            private TreeView tree;
            /// <summary>
            /// TODO: Temporarily using SpanID as keys (e.g. won't work with NullTracer impl) because
            /// we don't have any easy way to check equality of EventHookSpans
            /// </summary>
            private ConcurrentDictionary<string, TreeViewItem> itemsForSpans = new ConcurrentDictionary<string, TreeViewItem>();

            public StatefulClass(TreeView tree)
            {
                this.tree = tree;
            }

            public void TracerOnSpanSetTag(object sender, EventHookTracer.SetTagEventArgs e)
            {
                string eKey = e.Key;
                object eValue = e.Value;
                ISpan span = GlobalTracer.Instance.ActiveSpan;
                
                var treeItem = this.GetOrCreateViewItem(span);
                treeItem.Dispatcher.Invoke(
                    () =>
                    {
                        var treeViewItem = new TreeViewItem
                        {
                            Header = $"{GetDateTime()}SetTag | {eKey}: {eValue}"
                        };
                        treeItem.Items.Add(treeViewItem);
                        this.SignalItemUpdate(treeViewItem);
                    });
            }

            private static string GetDateTime()
            {
                return $"[{DateTime.Now:T}] ";
            }

            public void TracerOnSpanLog(object sender, EventHookTracer.LogEventArgs e)
            {
                IEnumerable<KeyValuePair<string, object>> keyValuePairs = e.Fields;
                DateTimeOffset dateTimeOffset = e.Timestamp;
                ISpan span = GlobalTracer.Instance.ActiveSpan;

                var treeItem = this.GetOrCreateViewItem(span);

                // since json isn't in this library yet
                var logString = string.Join(", ", keyValuePairs.Select(kvp => $"{kvp.Key}:{kvp.Value}"));

                treeItem.Dispatcher.Invoke(
                    () =>
                    {
                        var treeViewItem = new TreeViewItem
                        {
                            Header = $"{GetDateTime()}{logString}"
                        };
                        treeItem.Items.Add(
                            treeViewItem);
                        this.SignalItemUpdate(treeViewItem);
                    });
            }

            public void TracerOnSpanFinished(object sender, EventHookTracer.SpanLifecycleEventArgs e)
            {
                string eOperationName = e.OperationName;
                // TODO: EventHookTracer should not call Finished before the span is finished. Need -ing vs -ed distinction
                ISpan underlyingSpan = e.Span;

                // Real logic
                var finishedSpan = GlobalTracer.Instance.ActiveSpan;
                var treeItem = this.GetOrCreateViewItem(finishedSpan);
                treeItem.Dispatcher.Invoke(
                    () =>
                    {
                        var treeViewItem = new TreeViewItem
                        {
                            Header = $"{GetDateTime()}Finished Operation"
                        };
                        treeItem.Items.Add(
                            treeViewItem);
                        treeItem.IsExpanded = false;
                        treeItem.Header += $" {GetDateTime()}: Finished";
                        this.SignalItemUpdate(treeViewItem);
                    });

                this.UpdateStackSpanFinishing(underlyingSpan);
            }

            private void UpdateStackSpanFinishing(ISpan eSpan)
            {
                // TODO: Validate eSpan is what we expect?
                this.currentSpan.Value = new List<ISpan>(this.currentSpan.Value.Take(this.currentSpan.Value.Count - 1));
            }

            public void TracerOnSpanActivated(object sender, EventHookTracer.SpanLifecycleEventArgs e)
            {
                string eOperationName = e.OperationName;
                ISpan underlyingSpan = e.Span;

                this.UpdateStackSpanActivated(underlyingSpan);
                
                // Real logic
                var startedSpan = GlobalTracer.Instance.ActiveSpan;
                var treeItem = this.GetOrCreateViewItem(startedSpan);
                treeItem.Dispatcher.Invoke(
                    () =>
                    {
                        treeItem.Header = $"{eOperationName} {GetDateTime()}- ";
                        var treeViewItem = new TreeViewItem
                        {
                            Header = $"{GetDateTime()}Started Operation"
                        };
                        treeItem.Items.Add(
                            treeViewItem);
                        treeItem.IsExpanded = true;
                        this.SignalItemUpdate(treeViewItem);
                    });
            }

            private void SignalItemUpdate(TreeViewItem treeItem)
            {
                var brush = new SolidColorBrush(Colors.Red);
                treeItem.Foreground = brush;
                
                var colorAnimation = new ColorAnimation()
                {
                    To = Color.FromRgb(155, 0, 0),
                    Duration = new Duration(TimeSpan.FromMilliseconds(1000)),
                    //AutoReverse = true,
                    //RepeatBehavior = default(RepeatBehavior)
                };
                var colorAnimation2 = new ColorAnimation()
                {
                    To = Color.FromRgb(0, 0, 0),
                    Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                    //AutoReverse = true,
                    //RepeatBehavior = default(RepeatBehavior)
                };
                //Storyboard.SetTarget(colorAnimation, treeItem);
                Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(
                    "(0).(1)",
                    TreeViewItem.ForegroundProperty,
                    SolidColorBrush.ColorProperty));
                Storyboard.SetTargetProperty(colorAnimation2, new PropertyPath(
                    "(0).(1)",
                    TreeViewItem.ForegroundProperty,
                    SolidColorBrush.ColorProperty));

                Storyboard sb = new Storyboard();
                sb.Children.Add(colorAnimation);
                sb.Children.Add(colorAnimation2);
                sb.Begin(treeItem);

                if (treeItem.Parent is TreeViewItem)
                {
                    SignalItemUpdate(treeItem.Parent as TreeViewItem);
                }
            }

            private void UpdateStackSpanActivated(ISpan eSpan)
            {
                this.currentSpan.Value = new List<ISpan>(this.currentSpan.Value ?? Enumerable.Empty<ISpan>()) {eSpan};
            }

            private TreeViewItem GetOrCreateViewItem(ISpan span)
            {
                return this.itemsForSpans.GetOrAdd(
                    span.Context.SpanId,
                    (s) =>
                    {
                        var parentSpan = this.currentSpan.Value.ElementAtOrDefault(this.currentSpan.Value.Count - 2);
                        if (parentSpan == null)
                        {
                            return this.tree.Dispatcher.Invoke(
                                () =>
                                {
                                    TreeViewItem item = new TreeViewItem();
                                    this.tree.Items.Add(item);
                                    return item;
                                });
                        }
                        else
                        {
                            return this.tree.Dispatcher.Invoke(
                                () =>
                                {
                                    var parentViewItem = this.itemsForSpans[parentSpan.Context.SpanId];
                                    TreeViewItem item = new TreeViewItem();
                                    parentViewItem.Items.Add(item);
                                    this.SignalItemUpdate(parentViewItem);
                                    return item;
                                });
                        }
                    });
            }
        }
    }
}