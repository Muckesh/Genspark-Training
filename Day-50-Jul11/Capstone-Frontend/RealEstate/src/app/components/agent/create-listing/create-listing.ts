import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PropertyListingService } from '../../../services/property-listing.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PropertyImageService } from '../../../services/property-image.service';
import { Agent } from '../../../models/agent.model';
import { AuthService } from '../../../services/auth.service';
import { AgentService } from '../../../services/agent.service';
import { Alert } from '../../shared/alert/alert';

@Component({
  selector: 'app-create-listing',
  imports: [CommonModule,ReactiveFormsModule,Alert],
  templateUrl: './create-listing.html',
  styleUrl: './create-listing.css'
})
export class CreateListing implements OnInit{
  listingForm:FormGroup;
  isSubmitting=false;
  errorMessage: string | null = null;
  successMessage: string | null = null;
  selectedFiles:File[]=[];
  previewUrls: string[] = [];
  fileInputRef: HTMLInputElement | null = null;
  agents: Agent[]=[];
  isAdmin=false;



  constructor(private fb:FormBuilder,private listingService:PropertyListingService,public router:Router,private imageService:PropertyImageService,private authService:AuthService,private agentService:AgentService){
    this.listingForm=this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      description: ['', [Validators.required, Validators.minLength(20)]],
      price: [null, [Validators.required, Validators.min(1)]],
      location: ['', Validators.required],
      bedrooms: [null, [Validators.required, Validators.min(0)]],
      bathrooms: [null, [Validators.required, Validators.min(0)]],
      squareFeet: [null, [Validators.required, Validators.min(1)]],
      agentId:[null],
      propertyType: ['', Validators.required],
      listingType: ['', Validators.required],
      isPetsAllowed: ['', Validators.required],
      status: ['', Validators.required],
      hasParking: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.isAdmin=this.authService.hasRole("Admin");
    if (this.isAdmin) {
      this.loadAgents();
      this.listingForm.get('agentId')?.setValidators([Validators.required]);
    }
  }

  loadAgents(){
    this.agentService.getAllAgents().subscribe({
      next:(agents)=>{
        this.agents=agents.items;
      },
      error:(error)=>{
        this.errorMessage = 'Failed to load agents' + error;
      }
    });
  }
  
  onFileSelected(event: any) {
    const files = event.target.files;
    if (!files || files.length === 0) return;

    // Add new files to existing selection (up to 10 total)
    const newFiles = Array.from(files).slice(0, 10 - this.selectedFiles.length) as File[];

    this.selectedFiles = [...this.selectedFiles,...newFiles];
    
    
    // Create preview URLs for new files
    newFiles.forEach(file=>{
      const reader = new FileReader();
      reader.onload=(e:any)=>{
        this.previewUrls.push(e.target.result);
      };
      reader.readAsDataURL(file);
    });

    // Reset the file input to allow selecting the same files again
    if (event.target) {
      event.target.value = '';
    }


  }

  removeImage(index: number) {
    this.selectedFiles.splice(index, 1);
    this.previewUrls.splice(index, 1);
  }

  submit() {
    if (this.listingForm.invalid) {
      this.errorMessage = 'Please fill all required fields correctly';
      return;
    }

    if (this.selectedFiles.length === 0) {
      this.errorMessage = 'Please upload at least one image';
      return;
    }


    this.isSubmitting = true;
    this.errorMessage = null;
    this.successMessage = null;

    const formValue = { ...this.listingForm.value };
    formValue.isPetsAllowed = formValue.isPetsAllowed === 'true' || formValue.isPetsAllowed === true;
    formValue.hasParking = formValue.hasParking === 'true' || formValue.hasParking === true;


    // creating listing first
    this.listingService.createListing(formValue).subscribe({
      next: (listing) => {
        // uploading images
        const uploadObservables = this.selectedFiles.map(file=>{
          console.log(file);

          return this.imageService.uploadImage(listing.id,file)
        });

        // Track upload progress
        let uploadCount=0;
        let uploadErrors=0;

        uploadObservables.forEach(obs=>{
          obs.subscribe({
            next:()=>{
              uploadCount++;
              if(uploadCount===uploadObservables.length){
                if(uploadErrors>0){
                  this.errorMessage=`Listing created but ${uploadErrors} image ${uploadErrors > 1 ? 's' :''} failed to upload`;
                }else{
                  this.successMessage='Listing created successfully with all images!';
                }
                this.isAdmin? setTimeout(() => this.router.navigate(['/admin/listings']), 1500) :setTimeout(() => this.router.navigate(['/agent/listings']), 1500);
              }
            },
            error:(error)=>{
              console.error('Image upload error:', error);
              uploadCount++;
              uploadErrors++;
              if (uploadCount === uploadObservables.length) {
                if (uploadErrors === uploadObservables.length) {
                  this.errorMessage = 'Listing created but all images failed to upload';
                } else {
                  this.errorMessage = `Listing created but ${uploadErrors} image${uploadErrors > 1 ? 's' : ''} failed to upload`;
                }
                this.isAdmin? setTimeout(() => this.router.navigate(['/admin/listings']), 1500) :setTimeout(() => this.router.navigate(['/agent/listings']), 1500);
              }
            }
          });
        });

        
      },
      error: (error) => {
        this.errorMessage = error.message || 'Failed to create listing';
        this.isSubmitting = false;
      }
    });

  }

  get f() {
    return this.listingForm.controls;
  }

}
