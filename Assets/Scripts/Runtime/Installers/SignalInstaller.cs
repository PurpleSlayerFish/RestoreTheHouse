﻿using Runtime.Signals;
using Signals;
using Zenject;

namespace Runtime.Installers
{
    public class SignalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<SignalGameInit>();
            Container.DeclareSignal<SignalMakeHudButtonsVisible>();
            Container.DeclareSignal<SignalBlackScreen>();
            Container.DeclareSignal<SignalQuestionChoice>();
            Container.DeclareSignal<SignalRecipeUpdate>();
            Container.DeclareSignal<SignalResourceUpdate>();
            Container.DeclareSignal<SignalJoystickUpdate>();
        }
    }
}