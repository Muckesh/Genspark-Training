import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BrowseListings } from './browse-listings';

describe('BrowseListings', () => {
  let component: BrowseListings;
  let fixture: ComponentFixture<BrowseListings>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BrowseListings]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BrowseListings);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
