# Using OpenGL ULGPUDriver implementation in c/c++

## Linking

* todo

## Initialization

```cpp
// Initializes
// Arguments
// (maybe use true/false?)
// * uint32_t sampleCount (0 - 4xMSAA if available, 1 - no multisampling, any other number - sample count for RenderBuffers (not recommended))
Platform::instance().set_gpu_driver(UNGL_Initialize_GPUDriver()));
```

## Usage

```cpp

renderer->Update();
renderer->Render(); // modifes program, uniform buffer, vbo, fb, scissors, blend, blendfunc, viewport, depth

// struct UNGLRT {  uint32_t texture_id; uint32_t framebuffer_id; }

UNGLRT viewRt = UNGL_GetRT(view->Get()) // Texture2D and framebuffer it's bound to. Executes glBlit(MSAAFB, FB) behind scenes.
// UNGLRT viewRt = UNGL_GetMSAART(view->Get()) // Texture2DMultisample and framebuffer it's bound to.

// OpenGL texture id
uint32_t texture_id = viewRt.texture_id;

// use it

```
