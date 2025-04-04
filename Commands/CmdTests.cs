﻿using _ARK_;
using System.Collections.Generic;
using UnityEngine;

namespace _COBRA_
{
    internal static class CmdTests
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            Command.cmd_root_shell.AddCommand("load-scene", new Command(
                args: exe =>
                {
                    if (exe.line.TryReadArgument(out string scene_name, new string[] { "scene_test1", "scene_test2", "scene_test3", }))
                        exe.args.Add(scene_name);
                },
                action: exe =>
                {
                    Debug.Log($"Loading scene: {exe.args[0]}");
                    NUCLEOR.instance.scheduler.AddRoutine(Util.EWaitForSeconds(3, false, null));
                }),
                "LoadScene");

            Command.cmd_root_shell.AddCommand("test-options", new Command(
                args: exe =>
                {
                    if (exe.line.TryReadFlags(exe, out var flags, "-m", "--meaning", "--enhance"))
                        foreach (var flag in flags)
                            exe.args.Add(flag);
                    if (exe.line.TryReadArgument(out string arg, new[] { "bush", "fire", "word", }))
                        exe.args.Add(arg);
                },
                action: exe => exe.Stdout(exe.args.LinesToText())
                ));

            Command.cmd_root_shell.AddCommand("routine-test", new Command(
                args: exe =>
                {
                    if (exe.line.TryReadArgument(out string arg))
                        exe.args.Add(int.Parse(arg));
                },
                routine: ERoutineTest));

            static IEnumerator<CMD_STATUS> ERoutineTest(Command.Executor executor)
            {
                int loops = (int)executor.args[0];
                for (int i = 0; i < loops; ++i)
                {
                    executor.Stdout(i);
                    float timer = 0;
                    while (timer < 1)
                    {
                        timer += 3 * Time.deltaTime;
                        yield return new CMD_STATUS()
                        {
                            state = CMD_STATES.BLOCKING,
                            progress = (i + timer) / loops,
                        };
                    }
                }
            }
        }
    }
}