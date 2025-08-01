import { Component, OnInit } from '@angular/core';
import { ProductRequest, ProductResponse } from '../../../models/product.model';
import { CategoryResponse } from '../../../models/category.model';
import { ColorResponse } from '../../../models/color.model';
import { ModelResponse } from '../../../models/model.model';
import { CategoryService } from '../../../services/category.service';
import { ColorService } from '../../../services/color.service';
import { ModelService } from '../../../services/model.service';
import { ProductService } from '../../../services/product.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './product.html',
  styleUrls: ['./product.css']
})
export class Product implements OnInit {
  products: ProductResponse[] = [];
  categories: CategoryResponse[] = [];
  colors: ColorResponse[] = [];
  models: ModelResponse[] = [];
  
  productFormData: ProductRequest = {
    productName: '',
    image: null,
    price: 0,
    userId: undefined,
    categoryId: undefined,
    colorId: undefined,
    modelId: undefined,
    // sellStartDate: undefined,
    // sellEndDate: undefined,
    // isNew: undefined
  };
  
  isEditMode: boolean = false;
  editingProductId: number | null = null;
  selectedFile: File | null = null;
  previewImage: string | ArrayBuffer | null = null;
  currentImageUrl: string | null = null;
  showForm: boolean = false;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private colorService: ColorService,
    private modelService: ModelService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
    this.loadColors();
    this.loadModels();
  }

  showCreateForm(): void {
    this.resetForm();
    this.showForm = true;
    this.isEditMode = false;
  }

  loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (data) => this.products = data,
      error: (err) => console.error('Failed to load products', err)
    });
  }

  loadCategories(): void {
    this.categoryService.getAll().subscribe({
      next: (data) => this.categories = data,
      error: (err) => console.error('Failed to load categories', err)
    });
  }

  loadColors(): void {
    this.colorService.getAll().subscribe({
      next: (data) => this.colors = data,
      error: (err) => console.error('Failed to load colors', err)
    });
  }

  loadModels(): void {
    this.modelService.getAll().subscribe({
      next: (data) => this.models = data,
      error: (err) => console.error('Failed to load models', err)
    });
  }

  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      this.productFormData.image = file;
      this.currentImageUrl = null;

      const reader = new FileReader();
      reader.onload = () => {
        this.previewImage = reader.result;
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit(): void {
    if (!this.productFormData.image && !this.isEditMode) {
      alert('Please select an image');
      return;
    }

    if (this.isEditMode && this.editingProductId !== null) {
      const updateData: ProductRequest = {
        ...this.productFormData,
        image: this.productFormData.image || null
      };

      this.productService.update(this.editingProductId, updateData).subscribe({
        next: () => {
          this.loadProducts();
          this.resetForm();
          alert('Product updated successfully!');
        },
        error: (err) => {
          console.error('Update failed', err);
          alert(`Update failed: ${err.error?.message || err.message}`);
        }
      });
    } else {
      this.productService.create(this.productFormData).subscribe({
        next: () => {
          this.loadProducts();
          this.resetForm();
          alert('Product created successfully!');
        },
        error: (err) => {
          console.error('Create failed', err);
          alert(`Create failed: ${err.error?.message || err.message}`);
        }
      });
    }
  }

  // private createFileFromUrl(url: string | null): File | null {
  //   if (!url) return null;
  //   return new File([], 'existing-image.jpg', { type: 'image/jpeg' });
  // }

  editProduct(product: ProductResponse): void {
    this.isEditMode = true;
    this.showForm = true;
    this.editingProductId = product.productId;
    this.currentImageUrl = product.image;
    this.previewImage = product.image;
    
    this.productFormData = {
      productName: product.productName,
      price: product.price,
      image: null,
      userId: product.userId,
      categoryId: product.categoryId,
      colorId: product.colorId,
      modelId: product.modelId,
      // sellStartDate: product.sellStartDate ? new Date(product.sellStartDate) : undefined,
      // sellEndDate: product.sellEndDate ? new Date(product.sellEndDate) : undefined,
      // isNew: product.isNew
    };
  }

  deleteProduct(id: number): void {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.delete(id).subscribe({
        next: () => {
          this.loadProducts();
          alert('Product deleted successfully!');
        },
        error: (err) => {
          console.error('Delete failed', err);
          alert(`Delete failed: ${err.error?.message || err.message}`);
        }
      });
    }
  }

  getCategoryName(categoryId: number | undefined): string {
    if (!categoryId) return 'N/A';
    const category = this.categories.find(c => c.categoryId === categoryId);
    return category ? category.categoryName : 'N/A';
  }

  getColorName(colorId: number | undefined): string {
    if (!colorId) return 'N/A';
    const color = this.colors.find(c => c.colorId === colorId);
    return color ? color.colorName : 'N/A';
  }

  getModelName(modelId: number | undefined): string {
    if (!modelId) return 'N/A';
    const model = this.models.find(m => m.modelId === modelId);
    return model ? model.modelName : 'N/A';
  }

  resetForm(): void {
    this.isEditMode = false;
    this.showForm = false;
    this.editingProductId = null;
    this.productFormData = {
      productName: '',
      image: null,
      price: 0,
      userId: undefined,
      categoryId: undefined,
      colorId: undefined,
      modelId: undefined,
      // sellStartDate: undefined,
      // sellEndDate: undefined,
      // isNew: undefined
    };
    this.previewImage = null;
    this.currentImageUrl = null;
    this.selectedFile = null;
  }

  cancelForm(): void {
    this.resetForm();
  }
}