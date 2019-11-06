﻿namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;
    using static DataValidations;
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> entity)
        {
            entity
                    .HasKey(e => e.TeamId);

            entity
                .Property(e => e.Name)
                .HasMaxLength(TeamMaxNameLength)
                .IsRequired(true)
                .IsUnicode(true);

            entity
                .Property(e => e.LogoUrl)
                .HasMaxLength(LogoUrlMaxLength)
                .IsRequired(false)
                .IsUnicode(true);

            entity
                .Property(e => e.Initials)
                .HasMaxLength(InitialsUrlMaxLength)
                .IsRequired(false)
                .IsUnicode(true);

            entity
                .Property(e => e.Budget)
                .IsRequired(true);

            entity
                .HasOne(t => t.Town)
                .WithMany(to => to.Teams)
                .HasForeignKey(t => t.TownId)
                .HasConstraintName("FK_Teams_Towns")
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(t => t.PrimaryKitColor)
                .WithMany(pc => pc.PrimaryKitTeams)
                .HasForeignKey(t => t.PrimaryKitColorId)
                .HasConstraintName("FK_Teams_Colors_PrimaryTeams")
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(t => t.SecondaryKitColor)
                .WithMany(sc => sc.SecondaryKitTeams)
                .HasForeignKey(t => t.SecondaryKitColorId)
                .HasConstraintName("FK_Teams_Colors_SecondaryTeams")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
