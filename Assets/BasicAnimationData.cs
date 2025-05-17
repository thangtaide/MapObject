public class MapObjectAnimationData: BasicAnimationData
{
    public bool HasFallingAnimation;
    public bool HasLandedAnimation;
}

public class BasicAnimationData
{
    public BasicAnimationDirectionData[] Directions;
}

public class BasicAnimationDirectionData
{
    public BasicAnimationStateData[] States;
}

public class BasicAnimationStateData
{
    public float Duration;
    public BasicAnimationFrameData[] Frames;
}

public class BasicAnimationFrameData
{
    public string SpriteName;
    public float OffSetX;
    public float OffSetY;
    public int SortOrder;
}