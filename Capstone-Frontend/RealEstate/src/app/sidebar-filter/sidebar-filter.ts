import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-sidebar-filter',
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './sidebar-filter.html',
  styleUrl: './sidebar-filter.css'
})
export class SidebarFilter implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private destroy$=new Subject<void>();
  private filtersSubject = new Subject<any>();
  isCollapsed = false;


  
  filterForm = this.fb.group({
    location: [''],
    keyword: [''],
    minPrice: [''],
    maxPrice: [''],
    minBedrooms: [''],
    minBathrooms: ['']
  });

  @Output() filtersChanged = new EventEmitter<any>();

  ngOnInit(): void {
    // setup reactive pipeline
    this.filtersSubject.pipe(
      debounceTime(500),
      distinctUntilChanged((prev,curr)=>
      JSON.stringify(prev)===JSON.stringify(curr)
      ),
      takeUntil(this.destroy$)
    ).subscribe(filters=>{
      this.filtersChanged.emit(filters);
    });

    // watch for form changes
    this.filterForm.valueChanges.pipe(
      takeUntil(this.destroy$)
    ).subscribe(()=>{
      this.applyFilters();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  applyFilters() {
    const formValue = this.filterForm.value;
    const filters = {
      location:formValue.location||"",
      keyword: formValue.keyword || "",
      minPrice: formValue.minPrice ? Number(formValue.minPrice) : '',
      maxPrice: formValue.maxPrice ? Number(formValue.maxPrice) : '',
      minBedrooms: formValue.minBedrooms ? Number(formValue.minBedrooms) : '',
      minBathrooms: formValue.minBathrooms ? Number(formValue.minBathrooms) : ''
    };
    // this.filtersChanged.emit(filters);
    this.filtersSubject.next(filters);
  }

  toggleFilters() {
    this.isCollapsed = !this.isCollapsed;
  }

  resetFilters(){
    this.filterForm.reset({
      location:'',
      keyword:'',
      minPrice:'',
      maxPrice:'',
      minBedrooms:'',
      minBathrooms:''
    });
    // this.applyFilters();
  }
}
