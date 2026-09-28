using Planetbase;
using System;

namespace HarshWorld.Disasters.RogueBot
{
    internal class RogueBot : Specialization
    {
        public RogueBot()
        {
            mAi = RogueBotAi.getInstance();
            mCharacterType = typeof(Guest);
            mFlags = FlagAggressive | FlagGuest;
            mColor = CharacterDefinitions.getInstance().Guest.MainColor;
        }

        internal static void goRogue(Bot bot)
        {
            Specialization specializationRogueBot = null;
            if (bot.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Carrier>())
            {
                specializationRogueBot = RogueCarrier.getInstance();
            }
            else
            if (bot.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Constructor>())
            {
                specializationRogueBot = RogueConstructor.getInstance();
            }
            else
            if (bot.getSpecialization() == TypeList<Specialization, SpecializationList>.find<Driller>())
            {
                specializationRogueBot = RogueDriller.getInstance();
            }

            var rogueBot = Character.create(specializationRogueBot, bot.getPosition(), bot.getLocation());
            rogueBot.setRotation(bot.getRotation());
            bot.destroy();
        }

        internal static void restore(Character rogueBot)
        {
            if(rogueBot.isDead())
                return;

            Specialization specializationRogueBot = null;
            if (rogueBot.getSpecialization() is RogueCarrier)
            {
                specializationRogueBot = TypeList<Specialization, SpecializationList>.find<Carrier>();
            }
            else
            if (rogueBot.getSpecialization() is RogueConstructor)
            {
                specializationRogueBot = TypeList<Specialization, SpecializationList>.find<Constructor>();
            }
            else
            if (rogueBot.getSpecialization() is RogueDriller)
            {
                specializationRogueBot = TypeList<Specialization, SpecializationList>.find<Driller>();
            }

            var bot = Character.create(specializationRogueBot, rogueBot.getPosition(), rogueBot.getLocation());
            bot.setRotation(rogueBot.getRotation());
            rogueBot.destroy();
        }
    }
}
