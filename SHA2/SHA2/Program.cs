using SHA2;

//find error
//class for bit collection
//class for bit operations
//interfaces for abstraction

Console.WriteLine(new SHA256("1111111111111111111111").Hash());
//bb450a3c64ecfb7a95193d97d4a336642e2e4fbfa00c60a60d7a99bca316815d

Console.WriteLine(new SHA256("111111111111111111111").Hash());
//4aa6892909e369933b9f1babc10519121e2dfd1042551f6b9bdd4eae51f1f0c2

Console.WriteLine(new SHA256("1111111111111111111121").Hash());
//003f47e41434dc94a39a9e0ef09c2ff9b437e7cbfcff1e00970478f2b70ae349

