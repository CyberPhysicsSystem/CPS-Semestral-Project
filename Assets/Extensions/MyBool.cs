public static class MyBool
{
    public static bool Trigger(ref this bool trigger) =>
        trigger ? !(trigger = false) : false;
}