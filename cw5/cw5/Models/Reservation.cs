using System.ComponentModel.DataAnnotations;

namespace cw5.Models;

public class Reservation
{
   public int Id { get; set; }
   
   [Required]
   public int RoomId { get; set; }
   
   [Required]
   public string OrganizerName { get; set; }
   
   [Required]
   public string Topic { get; set; }
   
   [Required]
   public DateOnly Date { get; set; }
   
   [Required]
   public TimeSpan StartTime { get; set; }
   
   [Required]
   public TimeSpan EndTime { get; set; }
   
   [Required]
   public string Status { get; set; }

   public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
   {
      if (EndTime <= StartTime)
      {
         yield return new ValidationResult("EndTime must be after StartTime and EndTime", new[] { nameof(EndTime) });
      }
   }
   
}