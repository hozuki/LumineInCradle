namespace LumineInCradle;

public delegate void RefAction<T>(ref T arg);

public delegate void RefAction<T1, in T2>(ref T1 arg1, T2 arg2);
