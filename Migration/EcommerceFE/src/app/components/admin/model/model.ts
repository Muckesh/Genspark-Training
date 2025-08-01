import { Component, OnInit } from '@angular/core';
import { ModelRequest, ModelResponse } from '../../../models/model.model';
import { ModelService } from '../../../services/model.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-model',
  imports: [CommonModule,FormsModule,ReactiveFormsModule],
  templateUrl: './model.html',
  styleUrl: './model.css'
})
export class Model implements OnInit{
  models:ModelResponse[]=[];
  
    // form binding
    modelFormData:ModelRequest = {
      modelName:''
    };
    // edit mode
    isEditMode:boolean = false;
    editingModelId: number|null=null;
  
    constructor(private modelService:ModelService){}
  
    ngOnInit(): void {
      this.loadModels();
    }
  
    loadModels(){
      this.modelService.getAll().subscribe({
        next:(data)=>this.models=data,
        error:(error)=>console.error("Failed to load colors")
      });
    }
  
    onSubmit(){
      if (this.isEditMode&&this.editingModelId!=null) {
        this.modelService.update(this.editingModelId,this.modelFormData).subscribe({
          next:()=>{
            this.loadModels();
            this.resetForm();
          },
          error:(err)=>console.error('Update failed',err)
        });
      } else{
        this.modelService.create(this.modelFormData).subscribe({
          next:()=>{
            this.loadModels();
            this.resetForm();
          },
          error:(err)=>console.error('Create failed',err)
        });
      }
    }
  
    editModel(model:ModelResponse){
      this.isEditMode=true;
      this.editingModelId=model.modelId;
      this.modelFormData={modelName:model.modelName};
    }
  
    deleteModel(id:number){
      if(confirm("Are you sure you want to delete this model?")){
        this.modelService.delete(id).subscribe({
          next:()=>this.loadModels(),
          error:(err)=>console.error('Delete Failed',err)
        });
      }
    }
  
    resetForm(){
      this.isEditMode=false;
      this.editingModelId=null;
      this.modelFormData={modelName:''};
    }
}
