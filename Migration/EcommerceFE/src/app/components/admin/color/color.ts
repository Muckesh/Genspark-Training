import { Component, OnInit } from '@angular/core';
import { ColorRequest, ColorResponse } from '../../../models/color.model';
import { ColorService } from '../../../services/color.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-color',
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './color.html',
  styleUrl: './color.css'
})
export class Color implements OnInit {
  colors:ColorResponse[]=[];

  // form binding
  colorFormData:ColorRequest = {
    colorName:''
  };
  // edit mode
  isEditMode:boolean = false;
  editingColorId: number|null=null;

  constructor(private colorService:ColorService){}

  ngOnInit(): void {
    this.loadColors();
  }

  loadColors(){
    this.colorService.getAll().subscribe({
      next:(data)=>this.colors=data,
      error:(error)=>console.error("Failed to load colors")
    });
  }

  onSubmit(){
    if (this.isEditMode&&this.editingColorId!=null) {
      this.colorService.update(this.editingColorId,this.colorFormData).subscribe({
        next:()=>{
          this.loadColors();
          this.resetForm();
        },
        error:(err)=>console.error('Update failed',err)
      });
    } else{
      this.colorService.create(this.colorFormData).subscribe({
        next:()=>{
          this.loadColors();
          this.resetForm();
        },
        error:(err)=>console.error('Create failed',err)
      });
    }
  }

  editColor(color:ColorResponse){
    this.isEditMode=true;
    this.editingColorId=color.colorId;
    this.colorFormData={colorName:color.colorName};
  }

  deleteColor(id:number){
    if(confirm("Are you sure you want to delete this color?")){
      this.colorService.delete(id).subscribe({
        next:()=>this.loadColors(),
        error:(err)=>console.error('Delete Failed',err)
      });
    }
  }

  resetForm(){
    this.isEditMode=false;
    this.editingColorId=null;
    this.colorFormData={colorName:''};
  }

}
