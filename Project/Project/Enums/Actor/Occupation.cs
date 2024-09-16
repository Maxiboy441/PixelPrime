namespace Project.Enums.Occupation
{
    public enum Occupation
    {
        Actor,
        Animator,
        Editor,
        Cinematographer,
        FilmDirector,
        FilmProducer,
        FilmScoreComposer,
        Screenwriter,
        Publisher,
        TelevisionDirector,
        TelevisionProducer,
        Writer,
        VoiceActor
    }

    public static class OccupationEnumExtensions
    {
        public static Occupation? GetOccupation(string actorEnum)
        {
            return actorEnum switch
            {
                "actor" => Occupation.Actor,
                "animator" => Occupation.Animator,
                "cinematographer" => Occupation.Cinematographer,
                "editor" => Occupation.Editor,
                "film director" => Occupation.FilmDirector,
                "film producer" => Occupation.FilmProducer,
                "film score composer" => Occupation.FilmScoreComposer,
                "publisher" => Occupation.Publisher,
                "screenwriter" => Occupation.Screenwriter,
                "television director" => Occupation.TelevisionDirector,
                "television producer" => Occupation.TelevisionProducer,
                "writer" => Occupation.Writer,
                "voice actor" => Occupation.VoiceActor,
                _ => null
            };
        }
    }
}