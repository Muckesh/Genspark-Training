create or replace function proc_GetDoctorsBySpeciality(speciality varchar(30))
returns table(Id int, DoctorName text, YOE real)
as
$$
begin
	return query
	select distinct "Id", "Name", "YearsOfExperience" from public."Doctors"
	where "Id" in (select "DoctorId" from public."DoctorSpecialities" where "SpecialityId" in
	(select "Id" from public."Specialities" where "Name" = speciality));
end;
$$ Language plpgsql;

drop function proc_GetDoctorsBySpeciality(speciality varchar(30))

select distinct ("Id","Name","YearsOfExperience") from public."Doctors" 
where "Id" in (select "DoctorId" from public."DoctorSpecialities" where "SpecialityId" in 
(Select "Id" from public."Specialities" where "Name" = 'Cardiology'));

SELECT * FROM public."Doctors"
select * from public."Specialities" where "Name"='Cardiology'

select * from proc_GetDoctorsBySpeciality('Neurology')