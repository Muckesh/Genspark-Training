import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../../services/category.service';
import { CategoryRequest, CategoryResponse } from '../../../models/category.model';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-category',
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './category.html',
  styleUrl: './category.css'
})
export class Category implements OnInit {
  categories: CategoryResponse[] = [];

  // for form binding
  categoryFormData: CategoryRequest = { categoryName: '' };

  // for edit mode
  isEditMode: boolean = false;
  editingCategoryId: number | null = null;

  constructor(private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories = data,
      error: (err) => console.error('Failed to load categories', err)
    });
  }

  onSubmit(): void {
    if (this.isEditMode && this.editingCategoryId !== null) {
      this.categoryService.update(this.editingCategoryId, this.categoryFormData).subscribe({
        next: () => {
          this.loadCategories();
          this.resetForm();
        },
        error: (err) => console.error('Update failed', err)
      });
    } else {
      this.categoryService.create(this.categoryFormData).subscribe({
        next: () => {
          this.loadCategories();
          this.resetForm();
        },
        error: (err) => console.error('Create failed', err)
      });
    }
  }

  editCategory(category: CategoryResponse): void {
    this.isEditMode = true;
    this.editingCategoryId = category.categoryId;
    this.categoryFormData = { categoryName: category.categoryName };
  }

  deleteCategory(id: number): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.delete(id).subscribe({
        next: () => this.loadCategories(),
        error: (err) => console.error('Delete failed', err)
      });
    }
  }

  resetForm(): void {
    this.isEditMode = false;
    this.editingCategoryId = null;
    this.categoryFormData = { categoryName: '' };
  }
}
