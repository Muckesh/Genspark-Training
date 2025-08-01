export interface ProductRequest {
    productName: string;
    image: File|null;
    price: number;
    userId?: number;
    categoryId?: number;
    colorId?: number;
    modelId?: number;
    sellStartDate?: Date;
    sellEndDate?: Date;
    isNew?: number;
}

export interface ProductUpdateRequest {
    productName: string;
    image?: File|null;
    price: number;
    userId?: number;
    categoryId?: number;
    colorId?: number;
    modelId?: number;
    sellStartDate?: Date;
    sellEndDate?: Date;
    isNew?: number;
}

export interface ProductResponse {
    productId: number;
    productName: string;
    image: string;
    price: number;
    userId?: number;
    categoryId?: number;
    colorId?: number;
    modelId?: number;
    sellStartDate?: Date;
    sellEndDate?: Date;
    isNew?: number;
}