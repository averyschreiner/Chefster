using Chefster.Context;
using Chefster.Interfaces;
using Chefster.Models;
using Chefster.Common;
using Microsoft.Data.SqlClient;

namespace Chefster.Services;

public class FamilyService(FamilyDbContext context) : IFamily
{
    private readonly FamilyDbContext _context = context;

    public ServiceResult<FamilyModel> CreateFamily(FamilyModel family)
    {
        var fam = _context.FamilyModels.Find(family.id);

        if (fam != null)
        {
            return ServiceResult<FamilyModel>.ErrorResult("Family Already Exists");
        }

        family.createdAt = DateTime.UtcNow.ToString();

        try
        {
            _context.FamilyModels.Add(family);
            _context.SaveChanges(); // Save changes to database after altering it
            return ServiceResult<FamilyModel>.SuccessResult(family);
        }
        catch (SqlException e)
        {
            return ServiceResult<FamilyModel>.ErrorResult(
                $"Failed to write Family to database. Error: {e}"
            );
        }
    }

    public ServiceResult<FamilyModel> DeleteFamily(string id)
    {
        try
        {
            var fam = _context.FamilyModels.Find(id);
            if (fam == null)
            {
                return ServiceResult<FamilyModel>.ErrorResult("Family doesn't exist");
            }
            _context.Remove(fam);
            _context.SaveChanges();
            return ServiceResult<FamilyModel>.SuccessResult(fam);
        }
        catch (Exception e)
        {
            return ServiceResult<FamilyModel>.ErrorResult(
                $"Failed to remote family from database. Error: {e}"
            );
        }
    }

    public ServiceResult<List<FamilyModel>> GetAll()
    {
        try
        {
            return ServiceResult<List<FamilyModel>>.SuccessResult(_context.FamilyModels.ToList());
        }
        catch (SqlException e)
        {
            return ServiceResult<List<FamilyModel>>.ErrorResult(
                $"Failed to retrieve all families. Error: {e}"
            );
        }
    }

    public ServiceResult<FamilyModel?> GetById(string id)
    {
        try
        {
            return ServiceResult<FamilyModel?>.SuccessResult(_context.FamilyModels.Find(id));
        }
        catch (SqlException e)
        {
            return ServiceResult<FamilyModel?>.ErrorResult(
                $"Failed to retrieve all families. Error: {e}"
            );
        }
    }

    public ServiceResult<FamilyModel> UpdateFamily(FamilyModel family)
    {
        try
        {
            var existingFam = _context.FamilyModels.Find(family.id);
            if (existingFam == null)
            {
                return ServiceResult<FamilyModel>.ErrorResult("Family does not exist");
            }

            // update everything even if it wasnt changed. Not the most efficient, but works.
            existingFam.email = family.email;
            existingFam.phoneNumber = family.phoneNumber;
            existingFam.familySize = family.familySize;
            existingFam.members = family.members;
            existingFam.weeklyNotes = family.weeklyNotes;
            
            // we've edited the item in context, now just save it
            _context.SaveChanges();
            // return existing family since it should be the same as the new one "family"
            return ServiceResult<FamilyModel>.SuccessResult(existingFam);
        }
        catch (Exception e)
        {
            return ServiceResult<FamilyModel>.ErrorResult($"Failed to update Family. Error: {e}");
        }
    }
}
