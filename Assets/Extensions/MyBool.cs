public static class MyBool
{
    public static bool Trigger(this ref bool trigger)
    {
        if(trigger)
        {
            trigger = false;
            return true;
        }
        return false;
    }
}