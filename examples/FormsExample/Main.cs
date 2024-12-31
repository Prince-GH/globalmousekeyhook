// This code is distributed under the MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or https://mit-license.org/

using System;
using System.ComponentModel;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace Demo
{
    public partial class Main : Form
    {
        // Events for keyboard and mouse hooks
        private IKeyboardMouseEvents m_Events;

        // Constructor to initialize the form and set up initial state
        public Main()
        {
            InitializeComponent();
            radioGlobal.Checked = true; // Set default option to Global
            SubscribeGlobal(); // Subscribe to global keyboard and mouse events
            FormClosing += Main_Closing; // Attach event for form closing
        }

        // Event handler to handle form closing and unsubscribe from hooks
        private void Main_Closing(object sender, CancelEventArgs e)
        {
            Unsubscribe(); // Clean up subscriptions
        }

        // Subscribe to application-level keyboard and mouse events
        private void SubscribeApplication()
        {
            Unsubscribe(); // Unsubscribe from any existing events
            Subscribe(Hook.AppEvents()); // Attach application-specific events
        }

        // Subscribe to global-level keyboard and mouse events
        private void SubscribeGlobal()
        {
            Unsubscribe(); // Unsubscribe from any existing events
            Subscribe(Hook.GlobalEvents()); // Attach global-specific events
        }

        // Generic method to subscribe to events and attach handlers
        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events; // Store the event instance

            // Attach keyboard event handlers
            m_Events.KeyDown += OnKeyDown;
            m_Events.KeyUp += OnKeyUp;
            m_Events.KeyPress += HookManager_KeyPress;

            // Attach mouse event handlers
            m_Events.MouseUp += OnMouseUp;
            m_Events.MouseClick += OnMouseClick;
            m_Events.MouseDoubleClick += OnMouseDoubleClick;
            m_Events.MouseMove += HookManager_MouseMove;

            // Attach mouse drag event handlers
            m_Events.MouseDragStarted += OnMouseDragStarted;
            m_Events.MouseDragFinished += OnMouseDragFinished;

            // Handle mouse wheel events based on suppression settings
            if (checkBoxSupressMouseWheel.Checked)
            {
                m_Events.MouseWheelExt += HookManager_MouseWheelExt;
                m_Events.MouseHWheelExt += HookManager_MouseHWheelExt;
            }
            else
            {
                m_Events.MouseWheel += HookManager_MouseWheel;
                m_Events.MouseHWheel += HookManager_MouseHWheel;
            }

            // Handle mouse suppression settings
            if (checkBoxSuppressMouse.Checked)
                m_Events.MouseDownExt += HookManager_Supress;
            else
                m_Events.MouseDown += OnMouseDown;
        }

        // Unsubscribe from all events and release resources
        private void Unsubscribe()
        {
            if (m_Events == null) return;

            // Detach keyboard event handlers
            m_Events.KeyDown -= OnKeyDown;
            m_Events.KeyUp -= OnKeyUp;
            m_Events.KeyPress -= HookManager_KeyPress;

            // Detach mouse event handlers
            m_Events.MouseUp -= OnMouseUp;
            m_Events.MouseClick -= OnMouseClick;
            m_Events.MouseDoubleClick -= OnMouseDoubleClick;
            m_Events.MouseMove -= HookManager_MouseMove;

            // Detach mouse drag event handlers
            m_Events.MouseDragStarted -= OnMouseDragStarted;
            m_Events.MouseDragFinished -= OnMouseDragFinished;

            // Detach mouse wheel event handlers
            if (checkBoxSupressMouseWheel.Checked)
            {
                m_Events.MouseWheelExt -= HookManager_MouseWheelExt;
                m_Events.MouseHWheelExt -= HookManager_MouseHWheelExt;
            }
            else
            {
                m_Events.MouseWheel -= HookManager_MouseWheel;
                m_Events.MouseHWheel -= HookManager_MouseHWheel;
            }

            // Detach mouse suppression handlers
            if (checkBoxSuppressMouse.Checked)
                m_Events.MouseDownExt -= HookManager_Supress;
            else
                m_Events.MouseDown -= OnMouseDown;

            // Dispose of the events
            m_Events.Dispose();
            m_Events = null;
        }

        // Handler for suppressing mouse events
        private void HookManager_Supress(object sender, MouseEventExtArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                Log(string.Format("MouseDown \t\t {0}\n", e.Button));
                return;
            }

            Log(string.Format("MouseDown \t\t {0} Suppressed\n", e.Button));
            e.Handled = true; // Suppress the event
        }

        // Keyboard event handlers
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Log(string.Format("KeyDown  \t\t {0}\n", e.KeyCode));
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            Log(string.Format("KeyUp  \t\t\t {0}\n", e.KeyCode));
        }

        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            Log(string.Format("KeyPress \t\t\t {0}\n", e.KeyChar));
        }

        // Mouse event handlers
        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            labelMousePosition.Text = string.Format("x={0:0000}; y={1:0000}", e.X, e.Y); // Update mouse position label
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseDown \t\t {0}\n", e.Button));
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseUp \t\t {0}\n", e.Button));
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseClick \t\t {0}\n", e.Button));
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseDoubleClick \t\t {0}\n", e.Button));
        }

        private void OnMouseDragStarted(object sender, MouseEventArgs e)
        {
            Log("MouseDragStarted\n");
        }

        private void OnMouseDragFinished(object sender, MouseEventArgs e)
        {
            Log("MouseDragFinished\n");
        }

        // Mouse wheel event handlers
        private void HookManager_MouseWheel(object sender, MouseEventArgs e)
        {
            labelWheel.Text = string.Format("Wheel={0:000}", e.Delta); // Update wheel delta label
        }

        private void HookManager_MouseWheelExt(object sender, MouseEventExtArgs e)
        {
            labelWheel.Text = string.Format("Wheel={0:000}", e.Delta);
            Log("Mouse Wheel Move Suppressed.\n");
            e.Handled = true; // Suppress the event
        }

        private void HookManager_MouseHWheel(object sender, MouseEventArgs e)
        {
            labelHWheel.Text = string.Format("HWheel={0:000}", e.Delta); // Update horizontal wheel delta label
        }

        private void HookManager_MouseHWheelExt(object sender, MouseEventExtArgs e)
        {
            labelHWheel.Text = string.Format("HWheel={0:000}", e.Delta);
            Log("Horizontal Mouse Wheel Move Suppressed.\n");
            e.Handled = true; // Suppress the event
        }

        // Utility method to log messages in the text box
        private void Log(string text)
        {
            if (IsDisposed) return;
            textBoxLog.AppendText(text);
            textBoxLog.AppendText(Environment.NewLine);
            textBoxLog.ScrollToCaret();
        }

        // Event handlers for UI controls
        private void radioApplication_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) SubscribeApplication();
        }

        private void radioGlobal_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) SubscribeGlobal();
        }

        private void radioNone_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked) Unsubscribe();
        }

        private void checkBoxSupressMouseWheel_CheckedChanged(object sender, EventArgs e)
        {
            if (m_Events == null) return;

            // Adjust wheel event handlers based on suppression settings
            if (((CheckBox)sender).Checked)
            {
                m_Events.MouseWheel -= HookManager_MouseWheel;
                m_Events.MouseWheelExt += HookManager_MouseWheelExt;
                m_Events.MouseHWheel -= HookManager_MouseHWheel;
                m_Events.MouseHWheelExt += HookManager_MouseHWheelExt;
            }
            else
            {
                m_Events.MouseWheelExt -= HookManager_MouseWheelExt;
                m_Events.MouseWheel += HookManager_MouseWheel;
                m_Events.MouseHWheelExt -= HookManager_MouseHWheelExt;
                m_Events.MouseHWheel += HookManager_MouseHWheel;
            }
        }

        private void checkBoxSuppressMouse_CheckedChanged(object sender, EventArgs e)
        {
            if (m_Events == null) return;

            // Adjust mouse event handlers based on suppression settings
            if (((CheckBox)sender).Checked)
            {
                m_Events.MouseDown -= OnMouseDown;
                m_Events.MouseDownExt += HookManager_Supress;
            }
            else
            {
                m_Events.MouseDownExt -= HookManager_Supress;
                m_Events.MouseDown += OnMouseDown;
            }
        }
    }
}
