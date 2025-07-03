import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PropertyListingService } from '../../services/property-listing.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-edit-listing',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-listing.html',
  styleUrl: './edit-listing.css'
})
export class EditListing implements OnInit, OnDestroy {
  listingForm!: FormGroup;
  listingId: string | null = null;
  listingData: any = null;
  isEditing = false;
  updateMessage: string | null = null;
  updateError: string | null = null;
  isSubmitting = false;
  propertyTypes = ['Apartment', 'Independent House', 'Villa', 'Studio', 'Land'];
  listingTypes = ['Sale', 'Rent', 'Lease'];
  statusOptions = ['Available', 'Pending', 'Sold', 'Rented'];
  isAdmin=false;

  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private listingService: PropertyListingService,
    private authService:AuthService,
    private route: ActivatedRoute,
    public router: Router
  ) {}

  ngOnInit(): void {
    this.isAdmin=this.authService.hasRole("Admin");
    this.listingId = this.route.snapshot.paramMap.get('id');

    if (this.listingId) {
      this.listingService.getListingById(this.listingId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (listing) => {
            this.listingData = listing.items[0];
            this.initForm();
          },
          error: () => {
            this.updateError = 'Failed to load listing';
          }
        });
    }
  }

  initForm(): void {
    this.listingForm = this.fb.group({
      title: [this.listingData.title, [Validators.required, Validators.minLength(5)]],
      description: [this.listingData.description, [Validators.required, Validators.minLength(20)]],
      price: [this.listingData.price, [Validators.required, Validators.min(1)]],
      location: [this.listingData.location, Validators.required],
      bedrooms: [this.listingData.bedrooms, [Validators.required, Validators.min(0)]],
      bathrooms: [this.listingData.bathrooms, [Validators.required, Validators.min(0)]],
      squareFeet: [this.listingData.squareFeet, [Validators.required, Validators.min(1)]],
      propertyType: [this.listingData.propertyType, Validators.required],
      listingType: [this.listingData.listingType, Validators.required],
      isPetsAllowed: [this.listingData.isPetsAllowed || false,Validators.required],
      status: [this.listingData.status, Validators.required],
      hasParking: [this.listingData.hasParking || false,Validators.required]
    });
  }

  toggleEdit(): void {
    this.isEditing = !this.isEditing;
    if (this.isEditing && this.listingData) {
      this.listingForm.patchValue(this.listingData);
    }
  }

  onSubmit(): void {
    if (this.listingForm.invalid || !this.listingId) {
      this.updateError = 'Please correct form errors before submitting.';
      return;
    }

    this.isSubmitting = true;
    this.updateError = null;
    this.updateMessage = null;

    this.listingService.updateListing(this.listingId, this.listingForm.value)
      .subscribe({
        next: () => {
          this.updateMessage = 'Listing updated successfully!';
          this.isEditing = false;
          this.isSubmitting = false;
          this.isAdmin? setTimeout(() => this.router.navigate(['/admin/listings']), 1500) :setTimeout(() => this.router.navigate(['/agent/listings']), 1500);
        },
        error: () => {
          this.updateError = 'Failed to update listing.';
          this.isSubmitting = false;
        }
      });
  }

  get f() {
    return this.listingForm.controls;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
